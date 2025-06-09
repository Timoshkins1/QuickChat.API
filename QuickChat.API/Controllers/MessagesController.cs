using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuickChat.API.Data;
using QuickChat.API.Hubs;
using QuickChat.API.Models;

namespace QuickChat.API.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly ChatDbContext _db;
        private readonly IHubContext<ChatHub> _hub;

        public MessagesController(ChatDbContext db, IHubContext<ChatHub> hub)
        {
            _db = db;
            _hub = hub;
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetMessages(Guid chatId)
        {
            var messages = await _db.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.Text,
                    m.SenderName,
                    m.SenderId,
                    m.SentAt
                })
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Guid chatId, string username, string text)
        {
            var sender = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (sender == null)
                return BadRequest("Пользователь не найден");

            var chat = await _db.Chats.FirstOrDefaultAsync(c => c.Id == chatId);
            if (chat == null)
                return BadRequest("Чат не найден");

            var message = new Message
            {
                ChatId = chatId,
                SenderId = sender.Id,
                SenderName = sender.Name,
                Text = text,
                SentAt = DateTime.UtcNow
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            await _hub.Clients.Group(chatId.ToString())
                .SendAsync("ReceiveMessage", chatId.ToString(), text, sender.Name, sender.Id);

            return Ok("Сообщение отправлено");
        }
    }
}
