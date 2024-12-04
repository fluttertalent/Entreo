using System.ComponentModel.DataAnnotations;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitAnalytics
    {
        // Basic stats
        public int TotalCompletions { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }

        // Time-based analytics
        public DateTime? LastCompletedAt { get; set; }
        public double CompletionRate { get; set; } // Percentage of days completed
        public int DaysTracked { get; set; }

        // Period-specific completion counts
        public int CompletionsThisWeek { get; set; }
        public int CompletionsThisMonth { get; set; }
        public int CompletionsThisYear { get; set; }

        // Trends
        public bool IsImproving { get; set; } // Based on recent completion rate vs historical
        [MaxLength(10)]
        public string TrendDirection { get; set; }  // "Up", "Down", or "Stable"

        // Best performing periods
        public DayOfWeek MostProductiveDay { get; set; }
        public TimeSpan? MostProductiveTimeOfDay { get; set; }

        // Additional insights
        public Dictionary<string, int> CompletionsByDayOfWeek { get; set; }
        public Dictionary<string, double> MonthlyCompletionRates { get; set; }

        public HabitAnalytics()
        {
            CompletionsByDayOfWeek = new Dictionary<string, int>();
            MonthlyCompletionRates = new Dictionary<string, double>();
        }
    }
}