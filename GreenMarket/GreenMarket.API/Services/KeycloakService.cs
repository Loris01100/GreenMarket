using System.Net.Http.Headers;
using System.Text.Json;
using GreenMarket.Application.Interfaces;

namespace GreenMarket.API.Services;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _http;
    private readonly string _realm;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _baseUrl;

    public KeycloakService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _baseUrl = config["Keycloak:BaseUrl"]!;
        _realm = config["Keycloak:Realm"]!;
        _clientId = config["Keycloak:ClientId"]!;
        _clientSecret = config["Keycloak:ClientSecret"]!;
    }

    public async Task AssignerRoleAsync(Guid keycloakUserId, string role)
    {
        var token = await GetAdminTokenAsync();
        var roleRep = await GetRoleRepresentationAsync(role, token);
        await ModifyUserRolesAsync(keycloakUserId, roleRep, token, HttpMethod.Post);
    }

    public async Task SupprimerRoleAsync(Guid keycloakUserId, string role)
    {
        var token = await GetAdminTokenAsync();
        var roleRep = await GetRoleRepresentationAsync(role, token);
        await ModifyUserRolesAsync(keycloakUserId, roleRep, token, HttpMethod.Delete);
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var form = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret)
        ]);

        var response = await _http.PostAsync($"{_baseUrl}/realms/{_realm}/protocol/openid-connect/token", form);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString()!;
    }

    private async Task<JsonElement> GetRoleRepresentationAsync(string roleName, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"{_baseUrl}/admin/realms/{_realm}/roles/{roleName}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement.Clone();
    }

    private async Task ModifyUserRolesAsync(Guid userId, JsonElement roleRep, string token, HttpMethod method)
    {
        var body = JsonSerializer.Serialize(new[] { roleRep });
        using var request = new HttpRequestMessage(method,
            $"{_baseUrl}/admin/realms/{_realm}/users/{userId}/role-mappings/realm");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
