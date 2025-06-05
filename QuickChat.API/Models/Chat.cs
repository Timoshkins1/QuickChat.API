using System;
using System.Collections.Generic;

namespace QuickChat.API.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsGroup { get; set; }

        public ICollection<UserChat> UserChats { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}
