using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Shared.Models
{
    public class NotificationDelivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int NotificationScheduleId { get; set; }
        public virtual NotificationSchedule Schedule { get; set; }

        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? ReadAt { get; set; }

        public NotificationStatus Status { get; set; }

        [MaxLength(500)]
        public string ErrorMessage { get; set; }  // Details about delivery failure

        public int RetryCount { get; set; }
        public DateTime? NextRetryAt { get; set; }
    }

    public enum NotificationStatus
    {
        Pending,
        Sending,
        Delivered,
        Failed,
        Read,
        Cancelled
    }
}