using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace QuickChat.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001") // Замените на ваш адрес API
            };
        }

        public async Task<string> Login(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
            {
                Username = username,
                Password = password
            });

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}