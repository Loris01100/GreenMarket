namespace GreenMarket.Domain.Entities;

public class ChatMessage
{
    public Guid Id { get; set; }
    public string? SessionId { get; set; }
    public string? Sender { get; set; }
    public string? Text { get; set; }
    public DateTime Timestamp { get; set; }
}
