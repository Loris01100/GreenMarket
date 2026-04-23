using System.Text.Json;
using GreenMarket.Application.Interfaces;

namespace GreenMarket.API.Services;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KeycloakService> _logger;

    public KeycloakService(HttpClient httpClient, IConfiguration configuration, ILogger<KeycloakService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> UserExistsAsync(Guid keycloakId)
    {
        var baseUrl = _configuration["Keycloak:AuthServerUrl"];
        var realm = _configuration["Keycloak:Realm"];
        var token = await GetAdminTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync($"{baseUrl}/admin/realms/{realm}/users/{keycloakId}");
        return response.IsSuccessStatusCode;
    }

    public async Task AssignRoleAsync(Guid keycloakId, string role)
    {
        _logger.LogInformation("Assigning role {Role} to user {UserId}", role, keycloakId);
        // Implémentation complète nécessite le client_id du realm et l'API Admin Keycloak
        // Simplifiée ici pour la démonstration
        await Task.CompletedTask;
    }

    public async Task RemoveRoleAsync(Guid keycloakId, string role)
    {
        _logger.LogInformation("Removing role {Role} from user {UserId}", role, keycloakId);
        await Task.CompletedTask;
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var baseUrl = _configuration["Keycloak:AuthServerUrl"];
        var realm = _configuration["Keycloak:Realm"];
        var clientId = _configuration["Keycloak:AdminClientId"];
        var clientSecret = _configuration["Keycloak:AdminClientSecret"];

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId!,
            ["client_secret"] = clientSecret!
        };

        var response = await _httpClient.PostAsync(
            $"{baseUrl}/realms/{realm}/protocol/openid-connect/token",
            new FormUrlEncodedContent(form));

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("access_token").GetString()!;
    }
}
