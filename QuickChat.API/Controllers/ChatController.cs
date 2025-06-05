using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Data;
using QuickChat.API.DTO;
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
        [HttpPost("create")]
        public IActionResult CreateChat([FromBody] CreateChatRequest request)
        {
            var exists = _context.Chats.Any(c => c.Name.ToLower() == request.Name.ToLower());
            if (exists)
                return BadRequest("Чат с таким именем уже существует");

            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsGroup = true
            };

            _context.Chats.Add(chat);
            _context.SaveChanges();

            return Ok(chat);
        }

        // ✅ Подключить пользователя к чату
        [HttpPost("join")]
        public IActionResult JoinChat([FromBody] JoinChatRequest request)
        {
            var chat = _context.Chats.FirstOrDefault(c => c.Name.ToLower() == request.Name.ToLower());
            if (chat == null) return NotFound("Чат не найден");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, chat.PasswordHash))
                return Unauthorized("Неверный пароль");

            return Ok(chat);
        }

    }
}
