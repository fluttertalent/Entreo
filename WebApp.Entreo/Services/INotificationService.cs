using WebApp.Entreo.Shared.Models;

namespace WebApp.Entreo.Services
{
    public interface INotificationService
    {
        Task ScheduleNotificationAsync(NotificationSchedule schedule);
        Task SendImmediateNotificationAsync(string userId, string message, NotificationType type);
        Task ProcessPendingNotificationsAsync();
        Task HandleNotificationDeliveryStatusAsync(int deliveryId, NotificationStatus status);
        Task<List<NotificationDelivery>> GetUserNotificationsAsync(string userId, int page, int pageSize);
        Task MarkNotificationAsReadAsync(int deliveryId);
        Task UpdateUserPreferencesAsync(UserNotificationPreference preferences);
    }
}