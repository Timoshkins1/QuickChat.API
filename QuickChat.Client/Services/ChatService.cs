using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace QuickChat.Client.Services
{
    public class ChatService
    {
        private HubConnection? _connection;
        public event Action<string, string, string, Guid>? MessageReceived;

        public async Task Connect(string username)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5111/chatHub")
                .Build();

            _connection.On<string, string, string, Guid>("ReceiveMessage", (chatId, text, senderName, senderId) =>
            {
                MessageReceived?.Invoke(chatId, text, senderName, senderId);
            });

            await _connection.StartAsync();
            await _connection.InvokeAsync("JoinAllUserChats", username);
        }

        public async Task SendMessage(string chatId, string text, string senderName, Guid senderId)
        {
            if (_connection != null)
            {
                await _connection.InvokeAsync("SendMessage", chatId, text, senderName, senderId);
            }
        }

        public async Task JoinChatGroup(string chatId)
        {
            if (_connection != null)
            {
                await _connection.InvokeAsync("JoinChatGroup", chatId);
            }
        }
    }
}
