
using QuickChat.API.Data;
using QuickChat.API.Models;

namespace QuickChat.API.Services
{
    public class ChatService
    {
        private readonly ChatDbContext _context;

        public ChatService(ChatDbContext context)
        {
            _context = context;
        }

        public async Task<Chat> CreateChat(string name, Guid creatorId, bool isGroup = false)
        {
            var chat = new Chat
            {
                Name = name,
                IsGroup = isGroup
            };

            _context.Chats.Add(chat);

            _context.UserChats.Add(new UserChat
            {
                UserId = creatorId,
                Chat = chat
            });

            await _context.SaveChangesAsync();
            return chat;
        }
    }
}