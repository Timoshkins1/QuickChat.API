using System;
using System.Windows;
using QuickChat.Client.Services;
using QuickChat.Client.Views;

namespace QuickChat.Client
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService = new();
        private readonly ChatService _chatService = new(); // ⬅ создаём тут

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            var userIdString = await _apiService.LoginAsync(username, password);

            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
            {
                // ✅ Подключение к SignalR до открытия главного окна
                await _chatService.Connect(username);

                var main = new MainWindow(userId, username); // передаём в MainWindow
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ Ошибка входа. Проверьте логин и пароль.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new RegisterWindow();
            regWindow.ShowDialog();
        }
    }
}
