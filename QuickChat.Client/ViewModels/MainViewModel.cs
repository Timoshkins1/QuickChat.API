using QuickChat.Client.Services;
using System;
using System.Threading.Tasks;

namespace QuickChat.Client.ViewModels
{
    public class MainViewModel
    {
        private readonly ApiService _apiService;
        private readonly ChatService _chatService;

        private readonly Guid _userId;
        private readonly string _username;

        public MainViewModel(Guid userId, string username)
        {
            _userId = userId;
            _username = username;

            _apiService = new ApiService();
            _chatService = new ChatService();

            _ = InitializeAsync(); // запускаем подключение к SignalR
        }

        private async Task InitializeAsync()
        {
            await _chatService.Connect(_username);
        }

        // ты можешь вызвать этот метод из UI, если хочешь использовать VM:
        public async Task<Guid?> CreateChatWith(Guid targetUserId)
        {
            return await _apiService.CreatePrivateChatAsync(_userId, targetUserId);
        }
    }
}
