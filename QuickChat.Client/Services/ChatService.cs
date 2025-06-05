using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace QuickChat.Client.Services
{
    public class ChatService
    {
        private HubConnection _hubConnection;
        private string _username;

        public event Action<string, string, string> MessageReceived;

        public async Task Connect(string username)
        {
            _username = username;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5111/chatHub")
                .Build();

            _hubConnection.On<string, string, string>("ReceiveMessage", (chatId, message, sender) =>
            {
                MessageReceived?.Invoke(chatId, message, sender);
            });

            await _hubConnection.StartAsync();
        }

        public async Task SendMessage(Guid chatId, string message)
        {
            await _hubConnection.SendAsync("SendMessage", chatId, message, _username);
        }
    }
}
