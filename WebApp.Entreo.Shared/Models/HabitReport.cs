using System.ComponentModel.DataAnnotations;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitReport
    {
        // Basic habit information
        [Required]
        public int HabitId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Time period of the report
        [Required]
        public DateTime ReportStartDate { get; set; }

        [Required]
        public DateTime ReportEndDate { get; set; }

        // Overall statistics
        public int TotalCompletions { get; set; }
        public int TotalSkips { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? LastCompletedAt { get; set; }

        [Range(0, 100)]
        public double CompletionRate { get; set; }  // Percentage

        [Range(0, double.MaxValue)]
        public double AverageCompletionsPerDay { get; set; }

        [MaxLength(100)]
        public string MostActiveHabit { get; set; }
        public int MostActiveHabitCompletions { get; set; }

        [MaxLength(100)]
        public string MostSkippedHabit { get; set; }
        public int MostSkippedHabitCount { get; set; }

        public DateTime? MostProductiveDate { get; set; }
        public int MostProductiveDateCompletions { get; set; }

        // Complex metrics and analysis
        public List<HabitMetric> HabitMetrics { get; set; }
        public HabitTrends Trends { get; set; }
        public DetailedInsights DetailedInsights { get; set; }
        public MissedDaysAnalysis MissedDaysAnalysis { get; set; }

        // Periodic breakdowns
        public Dictionary<string, int> CompletionsByMonth { get; set; }      // "YYYY-MM" -> count
        public Dictionary<DayOfWeek, int> CompletionsByDayOfWeek { get; set; }
        public Dictionary<int, int> CompletionsByHour { get; set; }         // 0-23 -> count

        // Performance metrics
        public DayOfWeek MostProductiveDay { get; set; }
        public TimeSpan MostProductiveTimeOfDay { get; set; }

        [Range(0, int.MaxValue)]
        public int AverageCompletionsPerWeek { get; set; }

        [Range(0, int.MaxValue)]
        public int AverageCompletionsPerMonth { get; set; }

        // Trend analysis
        public bool IsImproving { get; set; }

        [MaxLength(20)]
        public string TrendDirection { get; set; }  // "Up", "Down", "Stable"

        [Range(0, 100)]
        public double RecentCompletionRate { get; set; }  // Last 30 days

        [Range(0, 100)]
        public double PreviousCompletionRate { get; set; } // 30 days before that

        // Missed days analysis
        public int TotalMissedDays { get; set; }
        public List<DateTime> LongestGapPeriod { get; set; }

        // Notes and observations
        [MaxLength(500)]
        public List<string> Insights { get; set; }

        [MaxLength(500)]
        public List<string> Recommendations { get; set; }

        // Time-based analysis
        public List<WeeklyTrend> WeeklyTrends { get; set; }
        public List<MonthlyGrowth> MonthlyGrowth { get; set; }

        public HabitReport()
        {
            CompletionsByMonth = new Dictionary<string, int>();
            CompletionsByDayOfWeek = new Dictionary<DayOfWeek, int>();
            CompletionsByHour = new Dictionary<int, int>();
            LongestGapPeriod = new List<DateTime>();
            Insights = new List<string>();
            Recommendations = new List<string>();
            HabitMetrics = new List<HabitMetric>();
            WeeklyTrends = new List<WeeklyTrend>();
            MonthlyGrowth = new List<MonthlyGrowth>();
        }
    }
}