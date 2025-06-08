public class MessageItem
{
    public string Text { get; set; }
    public bool IsMine { get; set; }
    public string Sender { get; set; } // ← вот это обязательно
}
