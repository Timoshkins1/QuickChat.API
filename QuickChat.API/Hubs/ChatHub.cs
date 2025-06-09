using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuickChat.API.Data;
using System.Text.RegularExpressions;

public class ChatHub : Hub
{
    private readonly ChatDbContext _db;

    // онлайн-подключения: username → connectionId
    private static readonly Dictionary<string, string> OnlineUsers = new();

    public ChatHub(ChatDbContext db)
    {
        _db = db;
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.GetHttpContext()?.Request.Query["username"];
        if (!string.IsNullOrEmpty(username))
        {
            OnlineUsers[username] = Context.ConnectionId;
            Console.WriteLine($"🔌 Подключен: {username}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = OnlineUsers.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        if (!string.IsNullOrEmpty(username))
        {
            OnlineUsers.Remove(username);
            Console.WriteLine($"🔌 Отключён: {username}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string chatId, string text, string senderName, Guid senderId)
    {
        await Clients.Group(chatId).SendAsync("ReceiveMessage", chatId, text, senderName, senderId);
    }

    public async Task JoinChatGroup(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
        Console.WriteLine($"✅ Присоединился к чату {chatId}");
    }

    public async Task JoinAllUserChats(string username)
    {
        var user = await _db.Users
            .Include(u => u.UserChats)
            .ThenInclude(uc => uc.Chat)
            .FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return;

        foreach (var uc in user.UserChats)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, uc.ChatId.ToString());
        }

        Console.WriteLine($"🔗 {username} присоединился ко всем чатам.");
    }

    // 🔥 Вызов из ChatController
    public async Task NotifyUserOfNewChat(string targetUsername, string chatId)
    {
        if (OnlineUsers.TryGetValue(targetUsername, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("NewChatCreated", chatId);
        }
    }
}
