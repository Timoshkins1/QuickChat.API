using QuickChat.API.Models;

public class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }   // внешний ключ
    public string SenderName { get; set; } // имя отправителя
    public string Text { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }

    public User Sender { get; set; }
    public Chat Chat { get; set; }
}
