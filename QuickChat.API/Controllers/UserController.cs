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
