// Models/UserChat.cs
using System;

namespace QuickChat.API.Models
{
    public class UserChat
    {
        public Guid UserId { get; set; }
        public Guid ChatId { get; set; }

        public User User { get; set; }
        public Chat Chat { get; set; }
    }
}
