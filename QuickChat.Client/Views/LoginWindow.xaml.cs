using System;
using System.Windows;
using QuickChat.Client.Services;
using QuickChat.Client.Views;

namespace QuickChat.Client
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService = new();

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
                var main = new MainWindow(userId, username);
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
