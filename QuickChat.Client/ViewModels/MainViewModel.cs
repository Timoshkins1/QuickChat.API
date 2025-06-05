using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickChat.Client.Services;
using System.Windows;

namespace QuickChat.Client.ViewModels
{
    internal class MainViewModel
    {
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var api = new ApiService();
            await api.CreateChatAsync("Новый чат");

            var chatService = new ChatService();
            await chatService.Connect("User1"); // без токена
            await chatService.SendMessage(Guid.NewGuid(), "Привет из клиента!");
        }

    }
}
