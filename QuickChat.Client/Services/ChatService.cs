using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace QuickChat.Client.Services
{
    public class ChatService
    {
        private HubConnection _hubConnection;

        public event Action<string, string> MessageReceived;

        public async Task Connect()
        {
            try
            {
                Console.WriteLine("🔌 Подключение к SignalR...");

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5111/chatHub") // ← важно
                    .Build();

                _hubConnection.On<string, string>("ReceiveMessage", (chatId, message) =>
                {
                    Console.WriteLine($"📩 Получено сообщение от чата {chatId}: {message}");
                    MessageReceived?.Invoke(chatId, message);
                });

                await _hubConnection.StartAsync();
                Console.WriteLine("✅ SignalR подключен");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Ошибка подключения к SignalR: " + ex.Message);
            }
        }

        public async Task SendMessage(Guid chatId, string message)
        {
            Console.WriteLine($"➡️ Отправка сообщения в чат {chatId}: {message}");
            await _hubConnection.SendAsync("SendMessage", chatId, message);
        }
    }
}
