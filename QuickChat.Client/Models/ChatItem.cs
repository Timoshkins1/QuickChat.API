using System;

namespace QuickChat.Client.Models
{
    public class ChatItem
    {
        public Guid Id { get; set; }
        public string OtherUser { get; set; } // Имя собеседника
    }
}
