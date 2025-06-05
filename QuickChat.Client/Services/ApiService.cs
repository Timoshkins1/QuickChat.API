using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace QuickChat.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5111");
        }

        public async Task CreateChatAsync(string name)
        {
            await _client.PostAsync($"/api/chats?name={Uri.EscapeDataString(name)}", null);
        }

        public async Task<List<string>> GetChatsAsync()
        {
            var response = await _client.GetAsync("/api/chats");
            return await response.Content.ReadFromJsonAsync<List<string>>() ?? new List<string>();
        }

        public async Task SendMessageToApiAsync(Guid chatId, string text)
        {
            await _client.PostAsync($"/api/messages?chatId={chatId}&text={Uri.EscapeDataString(text)}", null);
        }
    }
}
