namespace QuickChat.Client.Models
{
    public class MessageItem
    {
        public string Text { get; set; }
        public bool IsMine { get; set; } // ← новое
    }
}
