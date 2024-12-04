namespace WebApp.Entreo.Shared.Models
{
    public class WeekdayPerformance
    {
        // Basic metrics
        public DayOfWeek Day { get; set; }
        public int TotalCompletions { get; set; }
        public int TotalSkips { get; set; }
        public double CompletionRate { get; set; }
        public double ConsistencyScore { get; set; }

        // Time distribution
        public Dictionary<string, int> TimeOfDayDistribution { get; set; }  // "Morning", "Afternoon", etc.
        public string PeakPerformanceTime { get; set; }
        public Dictionary<int, int> HourlyBreakdown { get; set; }  // Hour -> Count

        // Habit performance
        public List<HabitDayPerformance> HabitPerformance { get; set; }
        public List<string> MostCompletedHabits { get; set; }
        public List<string> MostSkippedHabits { get; set; }

        // Streak information
        public int LongestStreak { get; set; }
        public double AverageStreak { get; set; }
        public int CurrentStreak { get; set; }

        // Pattern analysis
        public List<string> IdentifiedPatterns { get; set; }
        public Dictionary<string, double> SuccessFactors { get; set; }
        public Dictionary<string, double> ChallengeFactors { get; set; }

        public WeekdayPerformance()
        {
            TimeOfDayDistribution = new Dictionary<string, int>();
            HourlyBreakdown = new Dictionary<int, int>();
            HabitPerformance = new List<HabitDayPerformance>();
            MostCompletedHabits = new List<string>();
            MostSkippedHabits = new List<string>();
            IdentifiedPatterns = new List<string>();
            SuccessFactors = new Dictionary<string, double>();
            ChallengeFactors = new Dictionary<string, double>();
        }
    }

    public class HabitDayPerformance
    {
        public int HabitId { get; set; }
        public string HabitTitle { get; set; }
        public int CompletionCount { get; set; }
        public int SkipCount { get; set; }
        public double SuccessRate { get; set; }
        public string PreferredTimeBlock { get; set; }
        public List<string> CompletionTimes { get; set; }
        public Dictionary<string, double> Metrics { get; set; }

        public HabitDayPerformance()
        {
            CompletionTimes = new List<string>();
            Metrics = new Dictionary<string, double>();
        }
    }
}