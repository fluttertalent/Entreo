using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Shared.Models
{
    public class NotificationSchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int NotificationTemplateId { get; set; }
        public virtual NotificationTemplate Template { get; set; }

        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string UserId { get; set; }

        public int? HabitId { get; set; }
        public virtual Habit Habit { get; set; }

        public DateTime? ScheduledTime { get; set; }
        public bool IsRecurring { get; set; }

        [MaxLength(100)]
        public string CronExpression { get; set; }  // For complex scheduling

        public bool SendPush { get; set; }
        public bool SendEmail { get; set; }
        public bool IsEnabled { get; set; }
    }
}