namespace WebApp.Entreo.Shared.Models
{
    public class MonthlyGrowth
    {
        // Basic monthly metrics
        public DateTime Month { get; set; }
        public int CompletionCount { get; set; }
        public int SkipCount { get; set; }
        public double CompletionRate { get; set; }
        public double GrowthRate { get; set; }  // Percentage change from previous month

        // Detailed metrics
        public MonthlyMetrics Metrics { get; set; }

        // Habit-specific growth
        public List<HabitMonthlyProgress> HabitProgress { get; set; }

        // Pattern analysis
        public MonthlyPatternAnalysis Patterns { get; set; }

        public MonthlyGrowth()
        {
            Metrics = new MonthlyMetrics();
            HabitProgress = new List<HabitMonthlyProgress>();
            Patterns = new MonthlyPatternAnalysis();
        }
    }

    public class MonthlyMetrics
    {
        public int UniqueHabitsCompleted { get; set; }
        public int TotalDaysActive { get; set; }
        public double AverageCompletionsPerDay { get; set; }
        public int LongestStreak { get; set; }
        public Dictionary<DayOfWeek, double> WeekdayAverages { get; set; }
        public Dictionary<string, int> TimeOfDayDistribution { get; set; }

        public MonthlyMetrics()
        {
            WeekdayAverages = new Dictionary<DayOfWeek, double>();
            TimeOfDayDistribution = new Dictionary<string, int>();
        }
    }

    public class HabitMonthlyProgress
    {
        public int HabitId { get; set; }
        public string HabitTitle { get; set; }
        public int CompletionCount { get; set; }
        public int SkipCount { get; set; }
        public double CompletionRate { get; set; }
        public double GrowthFromPreviousMonth { get; set; }
        public int CurrentStreak { get; set; }
        public int BestStreak { get; set; }
        public List<DateTime> CompletionDates { get; set; }
        public string PerformanceIndicator { get; set; }  // "Improving", "Declining", "Stable"

        public HabitMonthlyProgress()
        {
            CompletionDates = new List<DateTime>();
        }
    }

    public class MonthlyPatternAnalysis
    {
        // Weekly patterns
        public Dictionary<int, WeekPerformance> WeeklyBreakdown { get; set; }  // Week number -> performance

        // Time patterns
        public string MostProductiveWeek { get; set; }
        public string MostProductiveDayOfWeek { get; set; }
        public string MostProductiveTimeOfDay { get; set; }

        // Consistency analysis
        public double ConsistencyScore { get; set; }
        public List<string> IdentifiedPatterns { get; set; }
        public Dictionary<string, double> CategoryPerformance { get; set; }

        public MonthlyPatternAnalysis()
        {
            WeeklyBreakdown = new Dictionary<int, WeekPerformance>();
            IdentifiedPatterns = new List<string>();
            CategoryPerformance = new Dictionary<string, double>();
        }
    }

    public class WeekPerformance
    {
        public int WeekNumber { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public int CompletionCount { get; set; }
        public int SkipCount { get; set; }
        public double CompletionRate { get; set; }
        public Dictionary<DayOfWeek, int> DailyCompletions { get; set; }
        public List<string> ActiveHabits { get; set; }

        public WeekPerformance()
        {
            DailyCompletions = new Dictionary<DayOfWeek, int>();
            ActiveHabits = new List<string>();
        }
    }
}