using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using QuickChat.Client.DTO;
using QuickChat.Client.Models;
using System.Text.Json;

namespace QuickChat.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

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
                return userId.Trim('"');
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Ошибка входа\nКод: {response.StatusCode}\nОтвет: {error}");
                return null;
            }
        }

        public async Task<List<ChatItem>> GetUserChatsAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/chats/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ChatItem>>(json, _jsonOptions) ?? new();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка при получении чатов: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение при получении чатов: {ex.Message}");
            }
            return new();
        }

        public async Task<Guid?> CreatePrivateChatAsync(Guid initiatorId, Guid recipientId)
        {
            var request = new CreatePrivateChatRequest
            {
                InitiatorId = initiatorId,
                RecipientId = recipientId
            };

            var response = await _httpClient.PostAsJsonAsync("/api/chats/create-private", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Guid>();
            }

            return null;
        }

        public async Task<List<MessageItem>> GetChatMessagesAsync(Guid chatId, Guid currentUserId)
        {
            var response = await _httpClient.GetAsync($"/api/messages/{chatId}");
            if (response.IsSuccessStatusCode)
            {
                var items = await response.Content.ReadFromJsonAsync<List<ChatMessageDto>>();
                return items.Select(m => new MessageItem
                {
                    Text = m.Text,
                    Sender = m.SenderName,
                    SenderId = m.SenderId,
                    IsMine = m.SenderId == currentUserId
                }).ToList();
            }

            return new List<MessageItem>();
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