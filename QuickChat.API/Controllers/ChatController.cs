using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Data;
using QuickChat.API.Models;
using Microsoft.EntityFrameworkCore;

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

        // 🔹 Получить все чаты конкретного пользователя
        [HttpGet("{userId}")]
        public IActionResult GetUserChats(Guid userId)
        {
            var chatIds = _context.UserChats
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.ChatId)
                .ToList();

            var chats = _context.Chats
                .Where(c => chatIds.Contains(c.Id))
                .Select(c => new
                {
                    c.Id,
                    OtherUser = _context.UserChats
                        .Where(uc => uc.ChatId == c.Id && uc.UserId != userId)
                        .Select(uc => uc.User.Name)
                        .FirstOrDefault()
                })
                .ToList();

            return Ok(chats);
        }

        // 🔹 Создать приватный чат между двумя пользователями (если его нет)
        [HttpPost("private")]
        public IActionResult CreatePrivateChat([FromQuery] Guid user1Id, [FromQuery] Guid user2Id)
        {
            var existingChatId = _context.UserChats
                .GroupBy(uc => uc.ChatId)
                .Where(g => g.Count() == 2 &&
                            g.Any(uc => uc.UserId == user1Id) &&
                            g.Any(uc => uc.UserId == user2Id))
                .Select(g => g.Key)
                .FirstOrDefault();

            if (existingChatId != Guid.Empty)
            {
                var existingChat = _context.Chats.FirstOrDefault(c => c.Id == existingChatId);
                return Ok(existingChat);
            }

            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                IsGroup = false
            };

            _context.Chats.Add(chat);
            _context.UserChats.Add(new UserChat { ChatId = chat.Id, UserId = user1Id });
            _context.UserChats.Add(new UserChat { ChatId = chat.Id, UserId = user2Id });

            _context.SaveChanges();

            return Ok(chat);
        }
    }
}
