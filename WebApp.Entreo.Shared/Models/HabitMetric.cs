using System.ComponentModel.DataAnnotations;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitMetric
    {
        public int HabitId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        public int TotalCompletions { get; set; }
        public int TotalSkips { get; set; }
        public double CompletionRate { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? LastCompletedAt { get; set; }

        // Time-based metrics
        public Dictionary<DayOfWeek, int> WeekdayDistribution { get; set; }
        public Dictionary<string, int> TimeOfDayDistribution { get; set; }
        public double WeekdayConsistency { get; set; }
        public double WeekendConsistency { get; set; }

        // Performance metrics
        public double AverageCompletionsPerWeek { get; set; }
        public double ConsistencyScore { get; set; }
        public int LongestGap { get; set; }
        public double AverageGap { get; set; }

        // Trend indicators
        [MaxLength(20)]
        public string TrendDirection { get; set; }  // "Improving", "Declining", "Stable"
        public double RecentCompletionRate { get; set; }  // Last 7 days
        public bool IsActive { get; set; }

        public HabitMetric()
        {
            WeekdayDistribution = new Dictionary<DayOfWeek, int>();
            TimeOfDayDistribution = new Dictionary<string, int>();
        }
    }
}