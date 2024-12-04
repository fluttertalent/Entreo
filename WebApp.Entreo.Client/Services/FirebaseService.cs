using Microsoft.JSInterop;

namespace WebApp.Entreo.Client.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ICustomHttpHelper _httpHelper;
        private readonly ILogger<FirebaseService> _logger;

        public FirebaseService(
            IJSRuntime jsRuntime,
            ICustomHttpHelper httpHelper,
            ILogger<FirebaseService> logger)
        {
            _jsRuntime = jsRuntime;
            _httpHelper = httpHelper;
            _logger = logger;
        }

        public async Task<string> RequestPermissionAsync()
        {
            try
            {
                _logger.LogInformation("Requesting notification permission");
                var permission = await _jsRuntime.InvokeAsync<string>(
                    "firebaseNotifications.requestPermission");
                _logger.LogInformation($"Permission result: {permission}");
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting notification permission");
                return "denied";
            }
        }

        public async Task<bool> CheckPermissionAsync()
        {
            try
            {
                var permission = await _jsRuntime.InvokeAsync<string>(
                    "firebaseNotifications.checkPermission");
                return permission == "granted";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking notification permission");
                return false;
            }
        }

        public async Task<string> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("firebaseNotifications.getToken");
        }

        public async Task SubscribeToTopicAsync(string topic)
        {
            var token = await GetTokenAsync();
            await _httpHelper.PostAsync($"api/notifications/subscribe/{topic}", token);
        }

        public async Task UnsubscribeFromTopicAsync(string topic)
        {
            var token = await GetTokenAsync();
            await _httpHelper.PostAsync($"api/notifications/unsubscribe/{topic}", token);
        }

        public async Task RegisterTokenAsync(string token)
        {
            await _httpHelper.PostAsync("api/notifications/register-token", new { Token = token });
        }
    }
}