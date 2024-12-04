using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Shared.Models
{
    public class NotificationTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // e.g., "StreakReminder", "DailyMotivation"

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }  // Email subject or notification title

        [Required]
        [MaxLength(10000)]  // Adjust max length based on your needs
        public string HtmlContent { get; set; }  // HTML version of the email content

        [MaxLength(5000)]  // Adjust max length based on your needs
        public string PlainTextContent { get; set; }  // Plain text version of the push notification / email content

        public NotificationType Type { get; set; }
        public NotificationPriority Priority { get; set; }

        [StringLength(100)]
        public string Category { get; set; }  // e.g., "Reminder", "Streak", "Achievement"

        public bool IsActive { get; set; }

        [MaxLength(255)]
        [Url]
        public string PreviewImage { get; set; }  // For push notifications
    }

    public enum NotificationType
    {
        Email,
        Push,
        Both
    }

    public enum NotificationPriority
    {
        Low,
        Normal,
        High,
        Urgent
    }
}