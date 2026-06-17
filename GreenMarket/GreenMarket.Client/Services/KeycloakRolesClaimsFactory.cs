using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace GreenMarket.Client.Services;

/// <summary>
/// Aplatit les rôles émis par Keycloak en claims <c>roles</c> individuels exploitables
/// par <c>AuthorizeView Roles="..."</c>.
///
/// Keycloak n'envoie pas de claim <c>roles</c> plat : les rôles arrivent soit imbriqués
/// dans <c>realm_access.roles</c> / <c>resource_access.&lt;client&gt;.roles</c>, soit (via un
/// mapper de rôles multivalué) sous forme d'un unique tableau JSON. Dans les deux cas Blazor
/// WASM ne sait pas les éclater, d'où des utilisateurs authentifiés mais sans rôle.
/// Cette factory récupère ces rôles et ajoute un <see cref="Claim"/> "roles" par valeur.
/// </summary>
public class KeycloakRolesClaimsFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    public KeycloakRolesClaimsFactory(IAccessTokenProviderAccessor accessor)
        : base(accessor)
    {
    }

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);

        if (user.Identity is not ClaimsIdentity identity)
            return user;

        // L'enrichissement des rôles ne doit JAMAIS faire échouer la pipeline d'auth :
        // une exception ici remonte jusqu'à AuthorizeView et fait disparaître toute l'UI
        // (y compris le bouton "Se connecter"). En cas de pépin, on dégrade : l'utilisateur
        // reste authentifié mais sans rôle plutôt que de planter l'application.
        try
        {
            // 1) Claim "roles" déjà présent mais sous forme de tableau JSON multivalué
            //    (ex: mapper Keycloak "User Realm Role" multivalué). On l'éclate.
            var rolesClaims = identity.FindAll("roles").ToList();
            foreach (var claim in rolesClaims)
            {
                foreach (var role in ParseRoles(claim.Value))
                {
                    if (!identity.HasClaim("roles", role))
                        identity.AddClaim(new Claim("roles", role));
                }
            }

            // 2) Rôles imbriqués dans realm_access.roles (présents si le token les inclut).
            AddNestedRoles(account, identity, "realm_access");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[KeycloakRolesClaimsFactory] enrichissement des rôles ignoré : {ex}");
        }

        return user;
    }

    private static void AddNestedRoles(
        RemoteUserAccount account,
        ClaimsIdentity identity,
        string accessKey)
    {
        if (account.AdditionalProperties is null ||
            !account.AdditionalProperties.TryGetValue(accessKey, out var value) ||
            value is null)
            return;

        try
        {
            var element = value is JsonElement je
                ? je
                : JsonSerializer.SerializeToElement(value);

            if (element.ValueKind == JsonValueKind.Object &&
                element.TryGetProperty("roles", out var roles) &&
                roles.ValueKind == JsonValueKind.Array)
            {
                foreach (var role in roles.EnumerateArray())
                {
                    var name = role.GetString();
                    if (!string.IsNullOrEmpty(name) && !identity.HasClaim("roles", name))
                        identity.AddClaim(new Claim("roles", name));
                }
            }
        }
        catch (Exception)
        {
            // Format inattendu : on ignore, l'utilisateur reste sans ce rôle.
        }
    }

    /// <summary>
    /// Un claim "roles" peut contenir une chaîne simple ("Producteur") ou un tableau
    /// JSON sérialisé ("[\"Producteur\",\"Admin\"]"). On gère les deux.
    /// </summary>
    private static IEnumerable<string> ParseRoles(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            yield break;

        var trimmed = value.TrimStart();
        if (trimmed.StartsWith('['))
        {
            JsonElement? parsed = null;
            try
            {
                parsed = JsonSerializer.Deserialize<JsonElement>(value);
            }
            catch (JsonException)
            {
                // Pas un JSON valide : on retombe sur la valeur brute plus bas.
            }

            if (parsed is { ValueKind: JsonValueKind.Array } array)
            {
                foreach (var item in array.EnumerateArray())
                {
                    var name = item.GetString();
                    if (!string.IsNullOrEmpty(name))
                        yield return name;
                }
                yield break;
            }
        }

        yield return value;
    }
}
