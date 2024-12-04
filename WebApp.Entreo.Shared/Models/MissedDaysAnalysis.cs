namespace WebApp.Entreo.Shared.Models
{
    public class MissedDaysAnalysis
    {
        // Basic metrics
        public int TotalMissedDays { get; set; }
        public double MissedDaysRate { get; set; }  // Percentage of total days
        public DayOfWeek MostMissedWeekday { get; set; }

        // Streak analysis
        public int LongestMissedStreak { get; set; }
        public double AverageGapBetweenMissedDays { get; set; }

        // Time-based breakdown
        public Dictionary<string, int> MonthlyMissedDays { get; set; }  // Key: "YYYY-MM"

        // Recent trends
        public MissedDaysTrend RecentMissedDaysTrend { get; set; }

        // Pattern analysis
        public List<MissedDayPattern> CommonPatterns { get; set; }

        // Impact analysis
        public HabitImpactAnalysis HabitImpact { get; set; }

        public MissedDaysAnalysis()
        {
            MonthlyMissedDays = new Dictionary<string, int>();
            CommonPatterns = new List<MissedDayPattern>();
        }
    }

    public class MissedDaysTrend
    {
        public int LastThirtyDaysMissed { get; set; }
        public int PreviousThirtyDaysMissed { get; set; }
        public string TrendDirection { get; set; }  // "Improving", "Declining", "Stable"
        public double ChangePercentage { get; set; }
    }

    public class MissedDayPattern
    {
        public string Pattern { get; set; }  // e.g., "Weekend", "Monday-Wednesday", "End of Month"
        public int Frequency { get; set; }
        public double Probability { get; set; }  // Likelihood of missing on this pattern
        public List<DateTime> OccurrenceDates { get; set; }
        public string PossibleCause { get; set; }

        public MissedDayPattern()
        {
            OccurrenceDates = new List<DateTime>();
        }
    }

    public class HabitImpactAnalysis
    {
        public Dictionary<int, MissedHabitImpact> ImpactByHabit { get; set; }
        public List<string> MostAffectedHabits { get; set; }
        public List<string> LeastAffectedHabits { get; set; }
        public Dictionary<string, double> CategoryImpact { get; set; }

        public HabitImpactAnalysis()
        {
            ImpactByHabit = new Dictionary<int, MissedHabitImpact>();
            MostAffectedHabits = new List<string>();
            LeastAffectedHabits = new List<string>();
            CategoryImpact = new Dictionary<string, double>();
        }
    }

    public class MissedHabitImpact
    {
        public int HabitId { get; set; }
        public string HabitTitle { get; set; }
        public int MissedCount { get; set; }
        public double ImpactScore { get; set; }  // 0-100 scale
        public List<string> RelatedHabitsAffected { get; set; }
        public string RecoveryPattern { get; set; }

        public MissedHabitImpact()
        {
            RelatedHabitsAffected = new List<string>();
        }
    }
}