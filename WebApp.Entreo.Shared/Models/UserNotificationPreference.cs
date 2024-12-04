using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Shared.Models
{
    public class UserNotificationPreference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string UserId { get; set; }
        public virtual User User { get; set; }

        // Communication channels
        public bool EnablePushNotifications { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public bool EnableSmsNotifications { get; set; }
        public bool EnableInAppNotifications { get; set; }

        // Digest preferences
        public bool EnableDailyDigest { get; set; }
        public bool EnableWeeklyReport { get; set; }
        public bool EnableMonthlyNewsletter { get; set; }

        // Quiet hours
        [Display(Name = "Quiet Hours Start")]
        public TimeSpan? QuietHoursStart { get; set; }

        [Display(Name = "Quiet Hours End")]
        public TimeSpan? QuietHoursEnd { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Time Zone")]
        public string TimeZoneId { get; set; }

        // Notification frequency controls
        [Range(0, 100)]
        [Display(Name = "Maximum Notifications Per Day")]
        public int MaxNotificationsPerDay { get; set; } = 10;

        [Range(0, 1440)] // Max minutes in a day
        [Display(Name = "Minimum Minutes Between Notifications")]
        public int MinTimeBetweenNotifications { get; set; } = 30;

        [Display(Name = "Preferred Notification Time")]
        public TimeSpan? PreferredNotificationTime { get; set; }

        // Category preferences
        public virtual ICollection<NotificationCategoryPreference> CategoryPreferences { get; set; }

        // Additional preferences
        public bool GroupSimilarNotifications { get; set; }
    }
}