namespace WebApp.Entreo.Shared.Models
{
    public class ProgressIndicators
    {
        // Overall progress
        public double OverallProgressScore { get; set; }  // 0-100
        public string ProgressTrend { get; set; }  // "Improving", "Declining", "Stable"
        public double ProgressChangeRate { get; set; }  // Percentage change

        // Time-based progress
        public Dictionary<string, double> WeeklyProgress { get; set; }  // "YYYY-WW" -> score
        public Dictionary<string, double> MonthlyProgress { get; set; }  // "YYYY-MM" -> score
        public List<ProgressPeriod> SignificantPeriods { get; set; }

        // Habit-specific progress
        public List<HabitProgressIndicator> HabitProgress { get; set; }

        // Achievement tracking
        public List<ProgressAchievement> RecentAchievements { get; set; }
        public Dictionary<string, double> MilestoneProgress { get; set; }

        // Growth metrics
        public GrowthMetrics Growth { get; set; }

        public ProgressIndicators()
        {
            WeeklyProgress = new Dictionary<string, double>();
            MonthlyProgress = new Dictionary<string, double>();
            SignificantPeriods = new List<ProgressPeriod>();
            HabitProgress = new List<HabitProgressIndicator>();
            RecentAchievements = new List<ProgressAchievement>();
            MilestoneProgress = new Dictionary<string, double>();
            Growth = new GrowthMetrics();
        }
    }

    public class HabitProgressIndicator
    {
        public int HabitId { get; set; }
        public string HabitTitle { get; set; }
        public double ProgressScore { get; set; }
        public string ProgressTrend { get; set; }
        public double CompletionRateChange { get; set; }
        public List<string> Improvements { get; set; }
        public List<string> Challenges { get; set; }
        public Dictionary<string, double> MetricChanges { get; set; }

        public HabitProgressIndicator()
        {
            Improvements = new List<string>();
            Challenges = new List<string>();
            MetricChanges = new Dictionary<string, double>();
        }
    }

    public class ProgressPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double ProgressRate { get; set; }
        public string Significance { get; set; }
        public List<string> KeyEvents { get; set; }
        public Dictionary<string, double> Metrics { get; set; }

        public ProgressPeriod()
        {
            KeyEvents = new List<string>();
            Metrics = new Dictionary<string, double>();
        }
    }

    public class ProgressAchievement
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateAchieved { get; set; }
        public string Category { get; set; }
        public int Value { get; set; }
        public List<string> RelatedHabits { get; set; }
        public Dictionary<string, double> ImpactMetrics { get; set; }

        public ProgressAchievement()
        {
            RelatedHabits = new List<string>();
            ImpactMetrics = new Dictionary<string, double>();
        }
    }

    public class GrowthMetrics
    {
        // Overall growth
        public double GrowthRate { get; set; }
        public string GrowthTrend { get; set; }

        // Time-based growth
        public Dictionary<string, double> WeeklyGrowth { get; set; }  // "YYYY-WW" -> rate
        public Dictionary<string, double> MonthlyGrowth { get; set; }  // "YYYY-MM" -> rate

        // Pattern analysis
        public List<GrowthPattern> IdentifiedPatterns { get; set; }
        public Dictionary<string, double> GrowthFactors { get; set; }

        // Projections
        public Dictionary<string, double> ShortTermProjections { get; set; }  // Next 30 days
        public Dictionary<string, double> LongTermProjections { get; set; }   // Next 90 days

        public GrowthMetrics()
        {
            WeeklyGrowth = new Dictionary<string, double>();
            MonthlyGrowth = new Dictionary<string, double>();
            IdentifiedPatterns = new List<GrowthPattern>();
            GrowthFactors = new Dictionary<string, double>();
            ShortTermProjections = new Dictionary<string, double>();
            LongTermProjections = new Dictionary<string, double>();
        }
    }

    public class GrowthPattern
    {
        public string PatternType { get; set; }
        public string Description { get; set; }
        public double Impact { get; set; }
        public List<string> ContributingFactors { get; set; }
        public Dictionary<string, double> TimeDistribution { get; set; }
        public bool IsActive { get; set; }

        public GrowthPattern()
        {
            ContributingFactors = new List<string>();
            TimeDistribution = new Dictionary<string, double>();
        }
    }
}