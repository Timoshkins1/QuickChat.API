using System;
using System.Threading.Tasks;
using QuickChat.Client.Services;

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

            _ = InitializeAsync(); // запускаем асинхронную инициализацию
        }

        private async Task InitializeAsync()
        {
            await _chatService.Connect(_username);
        }

        public async Task CreateChatAndSendTestMessage(Guid targetUserId, string targetUsername)
        {
            var chat = await _apiService.CreatePrivateChatAsync(_userId, targetUserId);

            if (chat != null)
            {
                await _chatService.SendMessage(chat.Id, $"Привет, {targetUsername}!");
                await _apiService.SendMessageToApiAsync(chat.Id, $"Привет, {targetUsername}!", _username);
            }
        }
    }
}
