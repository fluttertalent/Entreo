namespace WebApp.Entreo.Shared.Models
{
    public class HabitTrends
    {
        // Daily completion patterns
        public List<DailyCompletionPattern> DailyCompletions { get; set; }

        // Time-based pattern analysis
        public TimePatternAnalysis TimePatterns { get; set; }

        // Streak analysis
        public StreakAnalysis StreakPatterns { get; set; }

        // Consistency analysis
        public ConsistencyMetrics ConsistencyMetrics { get; set; }

        // Progress indicators
        public ProgressIndicators ProgressIndicators { get; set; }

        // Daily averages and patterns
        //public DailyAverages DailyAverages { get; set; }

        // Weekly patterns and comparisons
        public Dictionary<DayOfWeek, int> WeeklyPatterns { get; set; }

        // Monthly comparisons and growth
        //public List<MonthlyComparison> MonthlyComparisons { get; set; }

        public HabitTrends()
        {
            DailyCompletions = new List<DailyCompletionPattern>();
            TimePatterns = new TimePatternAnalysis();
            //TODO:
            //StreakPatterns = new StreakAnalysis();
            //ConsistencyMetrics = new ConsistencyMetrics();
            //ProgressIndicators = new ProgressIndicators();
            WeeklyPatterns = new Dictionary<DayOfWeek, int>();
            //MonthlyComparisons = new List<MonthlyComparison>();
        }
    }

    public class HabitProgressMetric
    {
        public int HabitId { get; set; }
        public string HabitTitle { get; set; }
        public double ProgressScore { get; set; }
        public string ProgressTrend { get; set; }
        public double CompletionRateChange { get; set; }
        public Dictionary<string, double> MetricChanges { get; set; }
        public List<string> Improvements { get; set; }
        public List<string> Challenges { get; set; }

        public HabitProgressMetric()
        {
            MetricChanges = new Dictionary<string, double>();
            Improvements = new List<string>();
            Challenges = new List<string>();
        }
    }

    public class TimeBasedProgress
    {
        public Dictionary<string, double> WeeklyProgress { get; set; }  // "YYYY-WW" -> score
        public Dictionary<string, double> MonthlyProgress { get; set; }  // "YYYY-MM" -> score
        public List<ProgressPeriod> SignificantPeriods { get; set; }
        public Dictionary<DayOfWeek, ProgressTrend> DayOfWeekProgress { get; set; }

        public TimeBasedProgress()
        {
            WeeklyProgress = new Dictionary<string, double>();
            MonthlyProgress = new Dictionary<string, double>();
            SignificantPeriods = new List<ProgressPeriod>();
            DayOfWeekProgress = new Dictionary<DayOfWeek, ProgressTrend>();
        }
    }

    public class AchievementMetrics
    {
        public int TotalAchievements { get; set; }
        public List<Achievement> RecentAchievements { get; set; }
        public Dictionary<string, int> AchievementsByCategory { get; set; }
        public List<string> NextMilestones { get; set; }
        public Dictionary<string, double> ProgressTowardsMilestones { get; set; }

        public AchievementMetrics()
        {
            RecentAchievements = new List<Achievement>();
            AchievementsByCategory = new Dictionary<string, int>();
            NextMilestones = new List<string>();
            ProgressTowardsMilestones = new Dictionary<string, double>();
        }
    }

    public class Achievement
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateAchieved { get; set; }
        public string Category { get; set; }
        public int Value { get; set; }
        public List<string> RelatedHabits { get; set; }

        public Achievement()
        {
            RelatedHabits = new List<string>();
        }
    }

    public class ProjectionMetrics
    {
        public Dictionary<string, double> ShortTermProjections { get; set; }  // Next 30 days
        public Dictionary<string, double> LongTermProjections { get; set; }   // Next 90 days
        public List<string> ProjectedMilestones { get; set; }
        public Dictionary<string, DateTime> EstimatedAchievementDates { get; set; }
        public List<string> ImprovementSuggestions { get; set; }

        public ProjectionMetrics()
        {
            ShortTermProjections = new Dictionary<string, double>();
            LongTermProjections = new Dictionary<string, double>();
            ProjectedMilestones = new List<string>();
            EstimatedAchievementDates = new Dictionary<string, DateTime>();
            ImprovementSuggestions = new List<string>();
        }
    }

    public class ProgressTrend
    {
        public string Direction { get; set; }
        public double Rate { get; set; }
        public List<string> Factors { get; set; }
        public Dictionary<string, double> Metrics { get; set; }

        public ProgressTrend()
        {
            Factors = new List<string>();
            Metrics = new Dictionary<string, double>();
        }
    }

    public class DailyCompletionPattern
    {
        public DateTime Date { get; set; }
        public int TotalCompletions { get; set; }
        public int SkippedCount { get; set; }
        public int UniqueHabits { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public Dictionary<string, int> TimeOfDayBreakdown { get; set; }
        public List<string> CompletedHabits { get; set; }

        public DailyCompletionPattern()
        {
            TimeOfDayBreakdown = new Dictionary<string, int>();
            CompletedHabits = new List<string>();
        }
    }

    public class TimePatternAnalysis
    {
        // Hourly distribution
        public Dictionary<int, int> HourlyDistribution { get; set; }

        // Peak completion times
        public int PeakCompletionHour { get; set; }
        public string PeakTimeOfDay { get; set; }  // "Morning", "Afternoon", "Evening", "Night"

        // Day of week patterns
        public Dictionary<DayOfWeek, int> WeekdayDistribution { get; set; }
        public DayOfWeek MostProductiveWeekday { get; set; }

        // Time block analysis
        public Dictionary<string, int> TimeBlockDistribution { get; set; }  // "Early Morning", "Morning", etc.
        public string MostProductiveTimeBlock { get; set; }

        // Consistency metrics
        public double TimeConsistencyScore { get; set; }
        public List<string> ConsistentTimeSlots { get; set; }

        // Weekend vs Weekday
        //public WeekendWeekdayAnalysis WeekendVsWeekday { get; set; }

        public TimePatternAnalysis()
        {
            HourlyDistribution = new Dictionary<int, int>();
            WeekdayDistribution = new Dictionary<DayOfWeek, int>();
            TimeBlockDistribution = new Dictionary<string, int>();
            ConsistentTimeSlots = new List<string>();
            //WeekendVsWeekday = new WeekendWeekdayAnalysis();
        }
    }

}