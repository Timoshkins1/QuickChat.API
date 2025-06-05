using System.Windows;
using QuickChat.Client.Models;
using QuickChat.Client.Services;

namespace QuickChat.Client.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly ApiService _apiService = new();

        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;
            string displayName = DisplayNameBox.Text;

            var success = await _apiService.RegisterAsync(username, password, displayName);

            if (success)
            {
                MessageBox.Show("✅ Аккаунт создан!");
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ Ошибка регистрации. Возможно, логин уже занят.");
            }
        }
    }
}
