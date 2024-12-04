namespace WebApp.Entreo.Shared.Models
{
    public class WeeklyTrends
    {
        // Basic weekly metrics
        public List<WeeklyTrend> WeeklyData { get; set; }

        // Week-over-week comparison
        public double WeekOverWeekGrowth { get; set; }

        // Best and worst weeks
        public WeeklyTrend BestWeek { get; set; }
        public WeeklyTrend WorstWeek { get; set; }

        // Weekly patterns
        public Dictionary<DayOfWeek, WeekdayPerformance> DayOfWeekAnalysis { get; set; }

        // Consistency metrics
        public double WeeklyConsistencyScore { get; set; }
        public string WeeklyTrendDirection { get; set; }  // "Improving", "Declining", "Stable"

        public WeeklyTrends()
        {
            WeeklyData = new List<WeeklyTrend>();
            DayOfWeekAnalysis = new Dictionary<DayOfWeek, WeekdayPerformance>();
        }
    }

    public class WeeklyTrend
    {
        public DateTime WeekStarting { get; set; }
        public int CompletionCount { get; set; }
        public int SkipCount { get; set; }
        public int UniqueHabits { get; set; }
        public double CompletionRate { get; set; }
        public Dictionary<DayOfWeek, int> DailyBreakdown { get; set; }
        public List<string> ActiveHabits { get; set; }
    }
}