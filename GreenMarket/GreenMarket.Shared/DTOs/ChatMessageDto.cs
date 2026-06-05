namespace GreenMarket.Shared.DTOs;

public record ChatMessageDto(
    Guid Id,
    string SessionId,
    string Sender,
    string Text,
    DateTime Timestamp
);
