using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Shared.Models
{
    public class NotificationCategoryPreference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int UserNotificationPreferenceId { get; set; }
        public virtual UserNotificationPreference UserPreference { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }  // e.g., "Reminders", "Achievements", "Streaks"

        public bool IsEnabled { get; set; }
    }
}