using GreenMarket.Application.Services;
using GreenMarket.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GreenMarket.API.Endpoints;

public static class ChatbotEndpoints
{
    public static void MapChatbotEndpoints(this WebApplication app)
    {
        app.MapPost("/api/chatbot", async (
            [FromBody] ChatMessageDto message,
            [FromServices] IChatbotService chatbotService) =>
        {
            var response = await chatbotService.ProcessMessageAsync(message);
            return Results.Ok(response);
        });
    }
}
