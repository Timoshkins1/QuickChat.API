using Microsoft.AspNetCore.Mvc;
using QuickChat.API.Models;

[ApiController]
[Route("api/chats")]
public class ChatController : ControllerBase
{
    [HttpGet]
    public IActionResult GetChats()
    {
        // Возвращаем список чатов пользователя
        return Ok(new List<Chat>());
    }

    [HttpPost]
    public IActionResult CreateChat(string name)
    {
        // Создаём новый чат
        return Ok("Чат создан!");
    }
}