using Microsoft.AspNetCore.SignalR.Client;

namespace QuickChat.Client.Services
{
    public class ChatService
    {
        private HubConnection _hubConnection;

        public event Action<string, string> MessageReceived;

        public async Task Connect(string token)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5001/chatHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                MessageReceived?.Invoke(user, message);
            });

            await _hubConnection.StartAsync();
        }

        public async Task SendMessage(Guid chatId, string message)
        {
            await _hubConnection.SendAsync("SendMessage", chatId, message);
        }
    }
}