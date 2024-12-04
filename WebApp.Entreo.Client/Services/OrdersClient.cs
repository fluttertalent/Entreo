using System.Net.Http.Json;
using WebApp.Entreo.Shared.Models;

namespace WebApp.Entreo.Client.Services
{
    public class OrdersClient
    {
        private readonly HttpClient _httpClient;

        public OrdersClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SubscribeToNotifications(NotificationSubscription subscription)
        {
            var response = await _httpClient.PostAsJsonAsync("WebPush/subscribe", subscription);
            response.EnsureSuccessStatusCode();
        }

        public async Task UnSubscribeToNotifications(string id)
        {
            var response = await _httpClient.PostAsJsonAsync("WebPush/unsubscribe", id);
            response.EnsureSuccessStatusCode();
        }
    }
}