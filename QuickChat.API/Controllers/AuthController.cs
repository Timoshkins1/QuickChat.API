using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Data;
using QuickChat.API.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace QuickChat.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public AuthController(ChatDbContext context)
        {
            _context = context;
        }

        // ✅ Регистрация
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Проверка: существует ли уже пользователь (без учёта регистра)
            var userExists = _context.Users.Any(u => u.Username.ToLower() == request.Username.ToLower());
            if (userExists)
                return BadRequest("Пользователь уже существует");

            // Хешируем пароль
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Name = request.DisplayName ?? request.Username,
                PasswordHash = hashedPassword,
                IsOnline = true,
                LastOnline = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { user.Id });
        }

        // ✅ Вход
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username.ToLower() == request.Username.ToLower());
            if (user == null)
                return Unauthorized("Пользователь не найден");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized("Неверный пароль");

            return Ok(user.Id); // 🔐 Можно заменить на JWT позже
        }
    }

    // 🔹 DTO для регистрации
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? DisplayName { get; set; }
    }

    // 🔹 DTO для входа
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
