using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

public class ChatHub : Hub
{
    public async Task JoinChat(Guid chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    public async Task SendMessage(Guid chatId, string message)
    {
        await Clients.Group(chatId.ToString())
            .SendAsync("ReceiveMessage", Context.User.Identity.Name, message);
    }
}