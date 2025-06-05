using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

[ApiController]
[Route("api/chats")]
public class ChatController : ControllerBase
{
    private static readonly List<string> ChatNames = new();

    [HttpGet]
    public IActionResult GetChats()
    {
        Console.WriteLine("📥 Запрос: получить список чатов");
        return Ok(ChatNames);
    }

    [HttpPost]
    public IActionResult CreateChat([FromQuery] string name)
    {
        Console.WriteLine($"🆕 Запрос: создать чат с именем '{name}'");
        ChatNames.Add(name);
        return Ok("Чат создан!");
    }
}
