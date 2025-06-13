using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Data;
using System.Linq;

namespace QuickChat.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public UserController(ChatDbContext context)
        {
            _context = context;
        }

        [HttpGet("{username}/isonline")]
        public IActionResult IsUserOnline(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
            if (user == null)
                return NotFound();

            var isOnline = user.LastOnline > DateTime.UtcNow.AddMinutes(-1);
            return Ok(isOnline);
        }
        [HttpGet("statuses")]
        public IActionResult GetUserStatuses()
        {
            var result = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    IsOnline = u.LastOnline > DateTime.UtcNow.AddMinutes(-1)
                })
                .ToList();

            return Ok(result);
        }

        // 🔍 Поиск пользователя по логину (без учёта регистра)
        [HttpGet("by-username")]
        public IActionResult GetByUsername([FromQuery] string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
                return NotFound("Пользователь не найден");

            return Ok(new
            {
                user.Id,
                user.Name
            });
        }
    }
}
