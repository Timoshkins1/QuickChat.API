using System.Windows;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace QuickChat.Client.Views
{
    public partial class JoinChatWindow : Window
    {
        private readonly HttpClient _httpClient;
        public Guid JoinedChatId { get; private set; }
        public string JoinedChatName { get; private set; }

        public JoinChatWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("http://localhost:5111/");
        }

        private async void JoinButton_Click(object sender, RoutedEventArgs e)
        {
            var chatName = ChatNameBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(chatName) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите имя и пароль чата.");
                return;
            }

            var request = new
            {
                Name = chatName,
                Password = password
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/chats/join", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var chat = JsonSerializer.Deserialize<JoinResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                JoinedChatId = chat.Id;
                JoinedChatName = chat.Name;

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка подключения к чату. Возможно, неверное имя или пароль.");
            }
        }

        private class JoinResponse
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}
