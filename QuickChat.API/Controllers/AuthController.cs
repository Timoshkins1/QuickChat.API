using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Data;
using QuickChat.API.Models;
using QuickChat.API.DTO;
using System;
using System.Linq;

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
            var userExists = _context.Users.Any(u => u.Username.ToLower() == request.Username.ToLower());
            if (userExists)
                return BadRequest("Пользователь уже существует");

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

            return Ok(user.Id);
        }

        // ✅ Вход
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null)
                return Unauthorized();

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized();

            return Ok(user.Id);
        }
    }
}
