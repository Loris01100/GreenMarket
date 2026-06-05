using GreenMarket.Shared.DTOs;

namespace GreenMarket.Application.Services;

public interface IChatbotService
{
    Task<ChatMessageDto> ProcessMessageAsync(ChatMessageDto message);
}
