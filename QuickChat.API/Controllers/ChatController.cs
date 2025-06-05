using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Data;
using QuickChat.API.Models;

namespace QuickChat.API.Controllers
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public ChatController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllChats()
        {
            var chats = _context.Chats.ToList();
            return Ok(chats);
        }

        [HttpPost]
        public IActionResult CreateChat([FromQuery] string name)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Name = name,
                IsGroup = true
            };

            _context.Chats.Add(chat);
            _context.SaveChanges();

            return Ok(chat); // возвращаем чат с Id
        }

        // ✅ Подключить пользователя к чату
        [HttpPost("join")]
        public IActionResult JoinChat([FromQuery] Guid chatId, [FromQuery] Guid userId)
        {
            var chat = _context.Chats.FirstOrDefault(c => c.Id == chatId);
            if (chat == null)
                return NotFound("Чат не найден");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound("Пользователь не найден");

            var alreadyInChat = _context.UserChats.Any(uc => uc.UserId == userId && uc.ChatId == chatId);
            if (alreadyInChat)
                return BadRequest("Пользователь уже в чате");

            _context.UserChats.Add(new UserChat
            {
                UserId = userId,
                ChatId = chatId
            });

            _context.SaveChanges();

            return Ok("Пользователь добавлен в чат");
        }
    }
}
