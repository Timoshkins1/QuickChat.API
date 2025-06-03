using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("register")]
    public IActionResult Register(string username, string password)
    {
        // Проверяем, есть ли пользователь
        // Хешируем пароль (BCrypt)
        // Сохраняем в базу данных
        return Ok("Пользователь создан!");
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        // Проверяем пароль
        // Генерируем JWT-токен (как ключ доступа)
        return Ok(new { Token = "ваш_токен" });
    }
}