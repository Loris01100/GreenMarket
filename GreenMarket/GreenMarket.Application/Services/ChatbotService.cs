using System.Net.Http.Json;
using System.Text.Json.Nodes;
using GreenMarket.Application.Services;
using GreenMarket.Shared.DTOs;
using Microsoft.Extensions.Configuration;

namespace GreenMarket.Application.Services;

public class ChatbotService : IChatbotService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ChatbotService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ChatMessageDto> ProcessMessageAsync(ChatMessageDto message)
    {
        var apiKey = _configuration["GoogleAi:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {   
            return new ChatMessageDto(Guid.NewGuid(), message.SessionId, "Bot", "Erreur : Clé d'API Google non configurée.", DateTime.UtcNow);
        }

        // Tentative avec l'API v1 stable et le modèle gemini-pro
        var model = "gemini-pro"; 
        var url = $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = message.Text }
                    }
                }
            }
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync(url, payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ChatMessageDto(Guid.NewGuid(), message.SessionId, "Bot", $"Erreur de l'API Google (v1) : {response.StatusCode} - {errorContent}", DateTime.UtcNow);
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonNode>();
            var generatedText = jsonResponse?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();

            return new ChatMessageDto(Guid.NewGuid(), message.SessionId, "Bot", generatedText ?? "Aucune réponse de l'IA.", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            return new ChatMessageDto(Guid.NewGuid(), message.SessionId, "Bot", $"Une erreur est survenue : {ex.Message}", DateTime.UtcNow);
        }
    }
}
