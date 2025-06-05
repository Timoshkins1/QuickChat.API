using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Data;
using QuickChat.API.Models;

namespace QuickChat.API.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public MessagesController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var messages = _context.Messages.ToList();
            return Ok(messages);
        }

        [HttpPost]
        public IActionResult SendMessage([FromQuery] Guid chatId, [FromQuery] string text)
        {
            // 1. Проверка существования чата
            var chatExists = _context.Chats.Any(c => c.Id == chatId);
            if (!chatExists)
                return BadRequest("❌ Чат с таким ID не существует");

            // 2. Проверка текста
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("❌ Сообщение не может быть пустым");

            // 3. Создание сообщения
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ChatId = chatId,
                SenderId = Guid.NewGuid(), // ❗ временно подставной пользователь
                Text = text,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(message);
            _context.SaveChanges();

            return Ok("✅ Сообщение успешно отправлено");
        }
    }
}
