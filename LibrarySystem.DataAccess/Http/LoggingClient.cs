using LibrarySystem.Application.Interfaces;
using System.Net.Http.Json;

namespace LibrarySystem.DataAccess.Http
{
    public class LoggingClient : ILoggingClient
    {
        private readonly HttpClient _httpClient;

        public LoggingClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LogAsync(string message, string createdBy)
        {
            var payload = new
            {
                Message = message,
                CreatedBy = createdBy
            };

            using var response = await _httpClient.PostAsJsonAsync("api/logs", payload);
            response.EnsureSuccessStatusCode();
        }
    }
}
