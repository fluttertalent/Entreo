using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Models
{
    // Tracks streak information for motivation
    public class HabitStreak
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Habit")]
        public int HabitId { get; set; }
        public virtual Habit Habit { get; set; }

        // Current Streak
        public int CurrentStreak { get; set; }
        public DateTime StreakStartDate { get; set; }
        public DateTime LastCompletedAt { get; set; }

        // Best Streak
        public int LongestStreak { get; set; }
        public DateTime LongestStreakStartDate { get; set; }
        public DateTime LongestStreakEndDate { get; set; }

        // Flexible Tracking
        public bool AllowSkipWeekends { get; set; }  // Don't break streak on weekends
        public bool AllowSkipHolidays { get; set; }  // Don't break streak on holidays
        public int MaxSkipsAllowed { get; set; }     // Number of skips before breaking streak
        public int SkipsUsed { get; set; }           // Current skip count

        // Recovery
        public int RecoveryDayCount { get; set; }    // Days to get back on track
        public DateTime? LastMissedDate { get; set; } // Last time streak was broken

        // Statistics
        public int TotalCompletions { get; set; }    // Total times completed
        public int TotalDaysMissed { get; set; }     // Total days missed
        public decimal CompletionRate { get; set; }   // Percentage of successful days
        public int WeeklyTarget { get; set; }        // Target days per week (e.g., 5 days/week)
    }
}