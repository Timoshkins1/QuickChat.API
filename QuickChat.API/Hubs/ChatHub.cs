using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    public async Task SendMessage(Guid chatId, string message)
    {
        Console.WriteLine($"💬 Получено сообщение в чат {chatId}: {message}");

        // Рассылаем сообщение всем
        await Clients.All.SendAsync("ReceiveMessage", chatId.ToString(), message);
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"✅ Подключился клиент: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"❌ Отключился клиент: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }
}
