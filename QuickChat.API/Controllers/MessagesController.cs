using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private static readonly List<(Guid ChatId, string Text)> Messages = new();

    [HttpPost]
    public IActionResult SendMessage(Guid chatId, string text)
    {
        Console.WriteLine($"📥 [API] Получено сообщение в чат {chatId}: {text}");
        Messages.Add((chatId, text));
        return Ok();
    }

    [HttpGet]
    public IActionResult GetMessages(Guid chatId)
    {
        var result = Messages.FindAll(m => m.ChatId == chatId);
        return Ok(result);
    }
}
