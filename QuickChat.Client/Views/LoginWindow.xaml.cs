using System.Windows;
using QuickChat.Client.Models;
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
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            var token = await _apiService.LoginAsync(username, password);

            if (token != null)
            {
                MessageBox.Show("✅ Успешный вход!");
                // Можно сохранить токен и открыть MainWindow
                var main = new MainWindow();
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
