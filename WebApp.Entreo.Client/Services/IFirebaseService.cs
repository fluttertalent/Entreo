namespace WebApp.Entreo.Client.Services
{
    public interface IFirebaseService
    {
        Task<string> RequestPermissionAsync();
        Task<string> GetTokenAsync();
        Task SubscribeToTopicAsync(string topic);
        Task UnsubscribeFromTopicAsync(string topic);
        Task RegisterTokenAsync(string token);
    }
}