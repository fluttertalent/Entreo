namespace WebApp.Entreo.Shared.Models
{
    public class StreakAnalysis
    {
        // Current streak information
        public int CurrentStreak { get; set; }
        public DateTime? CurrentStreakStartDate { get; set; }
        public bool IsCurrentlyActive { get; set; }

        // Historical streak data
        public int LongestStreak { get; set; }
        public DateTime? LongestStreakStartDate { get; set; }
        public DateTime? LongestStreakEndDate { get; set; }

        // Streak statistics
        public double AverageStreakLength { get; set; }
        public int TotalStreaks { get; set; }
        public Dictionary<int, int> StreakLengthDistribution { get; set; }  // Length -> Frequency

        // Time-based analysis
        public Dictionary<DayOfWeek, int> StreakStartDays { get; set; }
        public Dictionary<DayOfWeek, int> StreakEndDays { get; set; }
        public Dictionary<string, int> MonthlyStreakCounts { get; set; }  // "YYYY-MM" -> Count

        // Pattern analysis
        public List<StreakBreakPattern> BreakPatterns { get; set; }
        public List<StreakSuccessPattern> SuccessPatterns { get; set; }
        public Dictionary<string, double> StreakProbability { get; set; }  // Day pattern -> probability

        // Recovery metrics
        public double AverageRecoveryTime { get; set; }  // Days to start new streak
        public List<RecoveryPattern> RecoveryPatterns { get; set; }

        public StreakAnalysis()
        {
            StreakLengthDistribution = new Dictionary<int, int>();
            StreakStartDays = new Dictionary<DayOfWeek, int>();
            StreakEndDays = new Dictionary<DayOfWeek, int>();
            MonthlyStreakCounts = new Dictionary<string, int>();
            BreakPatterns = new List<StreakBreakPattern>();
            SuccessPatterns = new List<StreakSuccessPattern>();
            StreakProbability = new Dictionary<string, double>();
            RecoveryPatterns = new List<RecoveryPattern>();
        }
    }

    public class StreakBreakPattern
    {
        public string Pattern { get; set; }  // e.g., "Weekend Break", "Holiday Break"
        public int Frequency { get; set; }
        public double Probability { get; set; }
        public List<DateTime> OccurrenceDates { get; set; }
        public string PossibleCause { get; set; }
        public Dictionary<string, int> AssociatedHabits { get; set; }

        public StreakBreakPattern()
        {
            OccurrenceDates = new List<DateTime>();
            AssociatedHabits = new Dictionary<string, int>();
        }
    }

    public class StreakSuccessPattern
    {
        public string Pattern { get; set; }  // e.g., "Morning Routine", "Weekday Evening"
        public int Frequency { get; set; }
        public double SuccessRate { get; set; }
        public List<string> ContributingFactors { get; set; }
        public Dictionary<string, double> TimeDistribution { get; set; }
        public List<string> AssociatedHabits { get; set; }

        public StreakSuccessPattern()
        {
            ContributingFactors = new List<string>();
            TimeDistribution = new Dictionary<string, double>();
            AssociatedHabits = new List<string>();
        }
    }

    public class RecoveryPattern
    {
        public int DaysToRecover { get; set; }
        public string Pattern { get; set; }
        public double SuccessRate { get; set; }
        public List<string> EffectiveStrategies { get; set; }
        public Dictionary<DayOfWeek, int> StartDayDistribution { get; set; }
        public List<string> TriggerEvents { get; set; }

        public RecoveryPattern()
        {
            EffectiveStrategies = new List<string>();
            StartDayDistribution = new Dictionary<DayOfWeek, int>();
            TriggerEvents = new List<string>();
        }
    }
}