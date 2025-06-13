using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Data;
using QuickChat.API.Models;
using Microsoft.EntityFrameworkCore;
using QuickChat.API.DTO;

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

        [HttpGet("user/{userId}")]
        public IActionResult GetUserChats(Guid userId)
        {
            var chats = _context.UserChats
                .Where(uc => uc.UserId == userId)
                .Select(uc => new
                {
                    uc.Chat.Id,
                    OtherUser = uc.Chat.UserChats
                        .Where(ouc => ouc.UserId != userId)
                        .Select(ouc => ouc.User.Username)
                        .FirstOrDefault()
                })
                .ToList();

            var result = chats.Select(c =>
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == c.OtherUser);
                return new
                {
                    c.Id,
                    OtherUser = c.OtherUser,
                    IsOnline = user?.IsOnline ?? false
                };
            });

            return Ok(result);
        }


        [HttpPost("create-private")]
        public IActionResult CreatePrivateChat([FromBody] CreatePrivateChatRequest request)
        {
            var user1 = _context.Users.FirstOrDefault(u => u.Id == request.InitiatorId);
            var user2 = _context.Users.FirstOrDefault(u => u.Id == request.RecipientId);

            if (user1 == null || user2 == null)
                return BadRequest("Один из пользователей не найден");

            // Проверка: есть ли уже такой чат
            var existingChat = _context.Chats
                .Where(c => c.UserChats.Count == 2 &&
                            c.UserChats.Any(uc => uc.UserId == request.InitiatorId) &&
                            c.UserChats.Any(uc => uc.UserId == request.RecipientId))
                .FirstOrDefault();

            if (existingChat != null)
                return Ok(existingChat.Id); // Уже существует

            // Создаём новый чат с двумя записями UserChat
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                IsGroup = false,
                Messages = new List<Message>(),
                UserChats = new List<UserChat>
        {
            new UserChat { UserId = request.InitiatorId },
            new UserChat { UserId = request.RecipientId }
        }
            };

            _context.Chats.Add(chat);
            _context.SaveChanges();

            return Ok(chat.Id);
        }

    }
}