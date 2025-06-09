using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuickChat.API.Data;

namespace QuickChat.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _db;

        public ChatHub(ChatDbContext db)
        {
            _db = db;
        }

        public async Task SendMessage(string chatId, string text, string senderName, Guid senderId)
        {
            await Clients.Group(chatId).SendAsync("ReceiveMessage", chatId, text, senderName, senderId);
        }

        public async Task JoinChatGroup(string chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            Console.WriteLine($"✅ Подключение {Context.ConnectionId} к чату {chatId}");
        }

        public async Task JoinAllUserChats(string username)
        {
            var user = await _db.Users
                .Include(u => u.UserChats)
                .ThenInclude(uc => uc.Chat)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                Console.WriteLine($"❌ Пользователь {username} не найден для подключения к группам.");
                return;
            }

            foreach (var uc in user.UserChats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, uc.ChatId.ToString());
            }

            Console.WriteLine($"🔗 {username} присоединился ко всем чатам.");
        }
    }
}
