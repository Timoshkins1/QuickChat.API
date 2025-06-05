using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using QuickChat.Client.Models;

namespace QuickChat.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5111") // ⚠️ Убедись, что порт совпадает с твоим API
            };
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        public async Task<string> LoginAsync(string username, string password)
        {
            var request = new AuthRequest
            {
                Username = username,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); // Токен или UserId
            }

            return null;
        }
        public async Task<List<ChatItem>> GetChatsAsync()
        {
            var response = await _httpClient.GetAsync("/api/chats");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ChatItem>>();
            }

            return new List<ChatItem>();
        }
        public async Task<bool> SendMessageToApiAsync(Guid chatId, string text)
        {
            var response = await _httpClient.PostAsync($"/api/messages?chatId={chatId}&text={Uri.EscapeDataString(text)}", null);
            return response.IsSuccessStatusCode;
        }
        public async Task<ChatItem> CreateChatAsync(string name)
        {
            var response = await _httpClient.PostAsync($"/api/chats?name={Uri.EscapeDataString(name)}", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ChatItem>();
            }

            return null;
        }

        public async Task<bool> RegisterAsync(string username, string password, string displayName)
        {
            var request = new
            {
                Username = username,
                Password = password,
                DisplayName = displayName
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", request);
            return response.IsSuccessStatusCode;
        }
    }
}
