using System.Windows;
using QuickChat.Client.Services;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using QuickChat.Client.Models;

namespace QuickChat.Client.Views
{
    public partial class RegisterChatWindow : Window
    {
        private readonly HttpClient _httpClient;
        public Guid CreatedChatId { get; private set; }
        public string CreatedChatName { get; private set; }
        public RegisterChatWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new System.Uri("http://localhost:5111/");
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var chatName = ChatNameBox.Text;
            var password = PasswordBox.Password;

            var request = new { Name = chatName, Password = password };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/chats/create", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var chat = JsonSerializer.Deserialize<ChatResponse>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                CreatedChatId = chat.Id;
                CreatedChatName = chat.Name;

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при создании чата.");
            }
        }

    }
}
