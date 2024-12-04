using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitReminder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public int HabitId { get; set; }
        public virtual Habit Habit { get; set; }

        [Required]
        public int NotificationTemplateId { get; set; }
        public virtual NotificationTemplate NotificationTemplate { get; set; }

        [Required]
        [MaxLength(20)]
        public string NotificationType { get; set; } = "Push";  // Push, Email, SMS

        [Required]
        public TimeSpan Time { get; set; }

        // Bit flag for days of week (1-127)
        public byte DaysOfWeek { get; set; }

        public bool IsEnabled { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Helper methods for DaysOfWeek
        public bool IsScheduledForDay(DayOfWeek day)
        {
            return (DaysOfWeek & (1 << (int)day)) != 0;
        }

        public void SetDayOfWeek(DayOfWeek day, bool enabled)
        {
            if (enabled)
            {
                DaysOfWeek |= (byte)(1 << (int)day);
            }
            else
            {
                DaysOfWeek &= (byte)~(1 << (int)day);
            }
        }

        public bool ShouldRemindToday()
        {
            return IsEnabled && IsScheduledForDay(DateTime.UtcNow.DayOfWeek);
        }

        public DateTime GetNextReminderTime()
        {
            var now = DateTime.UtcNow;
            var today = now.Date;
            var reminderTimeToday = today.Add(Time);

            if (ShouldRemindToday() && now < reminderTimeToday)
            {
                return reminderTimeToday;
            }

            // Find next scheduled day
            for (int i = 1; i <= 7; i++)
            {
                var nextDay = today.AddDays(i);
                if (IsScheduledForDay(nextDay.DayOfWeek))
                {
                    return nextDay.Add(Time);
                }
            }

            return DateTime.MaxValue; // No upcoming reminders
        }
    }
}