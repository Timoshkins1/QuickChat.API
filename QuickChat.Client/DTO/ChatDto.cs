using System;

namespace QuickChat.Client.DTO
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public string OtherUser { get; set; }
        public bool IsOnline { get; set; }
    }
}
