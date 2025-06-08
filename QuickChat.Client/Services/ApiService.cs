using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using QuickChat.Client.DTO;
using QuickChat.Client.Models;

namespace QuickChat.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:5111");
        }

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
                var userId = await response.Content.ReadAsStringAsync();
                return userId.Trim('"'); // на всякий случай удаляем кавычки
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                System.Windows.MessageBox.Show($"Ошибка входа\nКод: {response.StatusCode}\nОтвет: {error}");
                return null;
            }
        }




        public async Task<List<ChatItem>> GetUserChatsAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/api/chats/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ChatItem>>();
            }

            return new List<ChatItem>();
        }

        public async Task<ChatItem> CreatePrivateChatAsync(Guid user1Id, Guid user2Id)
        {
            var response = await _httpClient.PostAsync($"/api/chats/private?user1Id={user1Id}&user2Id={user2Id}", null);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ChatItem>();
            }

            return null;
        }

        public async Task<bool> SendMessageToApiAsync(Guid chatId, string text, string username)
        {
            var response = await _httpClient.PostAsync($"/api/messages?chatId={chatId}&username={username}&text={Uri.EscapeDataString(text)}", null);
            return response.IsSuccessStatusCode;
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
