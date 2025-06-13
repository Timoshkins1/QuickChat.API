using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace QuickChat.Client.Views
{
    public partial class SelectUserWindow : Window
    {
        public Guid SelectedUserId { get; private set; }
        public string SelectedUserName { get; private set; }

        private readonly HttpClient _httpClient = new()
        {
            BaseAddress = new Uri("http://26.207.53.154:5111") // измени на нужный порт, если другой
        };

        public SelectUserWindow()
        {
            InitializeComponent();
        }

        private async void CreateChat_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Введите логин.");
                return;
            }

            var response = await _httpClient.GetAsync($"/api/users/by-username?username={login}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserDto>(
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // ➕ ПРОВЕРКА: в сети ли пользователь
                var statusResponse = await _httpClient.GetAsync($"/api/users/{login}/isonline");
                bool isOnline = false;
                if (statusResponse.IsSuccessStatusCode)
                {
                    var statusStr = await statusResponse.Content.ReadAsStringAsync();
                    isOnline = bool.Parse(statusStr);
                }

                // ➕ Показываем результат
                MessageBox.Show($"{user.Name} {(isOnline ? "в сети" : "не в сети")}");

                SelectedUserId = user.Id;
                SelectedUserName = user.Name;

                DialogResult = true;
                Close();
            }
            else
            {
                var msg = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Пользователь не найден.\n\nОтвет сервера: {msg}");
            }
        }


        private class UserDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
