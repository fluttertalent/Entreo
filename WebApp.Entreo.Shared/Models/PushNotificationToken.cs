using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Shared.Models
{
    public class PushNotificationToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string UserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Token { get; set; }  // Device-specific push notification token

        [Required]
        [StringLength(50)]
        public string DeviceId { get; set; }  // Unique device identifier

        [StringLength(100)]
        public string DeviceName { get; set; }  // User-friendly device name

        public DateTime LastSuccessfulDelivery { get; set; }
        public int FailedAttempts { get; set; }

        [StringLength(50)]
        public string DeviceTimeZone { get; set; }

        [StringLength(20)]
        public string Platform { get; set; }  // iOS, Android, Web

        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public bool IsActive { get; set; }
    }

}

