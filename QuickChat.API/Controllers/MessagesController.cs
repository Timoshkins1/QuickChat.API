using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuickChat.API.Data;
using QuickChat.API.Models;

namespace QuickChat.API.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly ChatDbContext _db;
        private readonly ChatDbContext _context;

        public MessagesController(ChatDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var messages = _context.Messages.ToList();
            return Ok(messages);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> SendMessage(Guid chatId, string username, string text)
        {
            // Найти пользователя по логину
            var sender = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (sender == null)
                return BadRequest("Пользователь не найден");

            // Проверка, что чат существует
            var chatExists = await _db.Chats.AnyAsync(c => c.Id == chatId);
            if (!chatExists)
                return BadRequest("Чат не найден");

            // Создаём сообщение
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

            return Ok("Сообщение отправлено");
        }
    }
}
