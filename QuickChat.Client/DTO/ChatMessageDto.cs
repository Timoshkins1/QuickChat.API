using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickChat.Client.DTO
{
    public class ChatMessageDto
    {
        public string Text { get; set; }
        public string SenderName { get; set; }
        public Guid SenderId { get; set; }
        public DateTime SentAt { get; set; }
    }

}
