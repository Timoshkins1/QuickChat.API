namespace QuickChat.Client.Models
{
    public class MessageItem
    {
        public string Text { get; set; }
        public bool IsMine { get; set; }
        public string Sender { get; set; }
        public Guid SenderId { get; set; }

        // Новое свойство для цвета отправителя
        public string SenderColor
        {
            get => IsMine ? "#FF8C00" : GenerateColorFromGuid(SenderId);
        }

        private static string GenerateColorFromGuid(Guid id)
        {
            var hash = id.GetHashCode();
            var random = new Random(hash);
            return $"#{random.Next(0x99, 0xEE):X2}{random.Next(0x99, 0xEE):X2}{random.Next(0x99, 0xEE):X2}";
        }
    }
}