using System.ComponentModel.DataAnnotations;

namespace WebApp.Entreo.Shared.Models
{
    /// <summary>
    /// Tracks and analyzes habit consistency patterns and metrics
    /// </summary>
    public class ConsistencyMetrics
    {
        [Range(0, 100)]
        public double OverallConsistencyScore { get; set; }  // 0-100

        [MaxLength(20)]
        public string ConsistencyTrend { get; set; }  // "Improving", "Declining", "Stable"

        [Range(-100, 100)]
        public double ConsistencyChangeRate { get; set; }  // Percentage change

        public Dictionary<DayOfWeek, double> DailyConsistencyScores { get; set; }
        public Dictionary<string, double> TimeBlockConsistency { get; set; }  // "Morning", "Afternoon", etc.
        public WeekendWeekdayMetrics WeekendVsWeekday { get; set; }
        public List<ConsistencyPattern> IdentifiedPatterns { get; set; }
        public Dictionary<string, double> SuccessFactors { get; set; }
        public List<InconsistencyTrigger> CommonTriggers { get; set; }
        public StreakConsistencyMetrics StreakMetrics { get; set; }
        public List<ConsistencyRecoveryPattern> RecoveryPatterns { get; set; }

        public ConsistencyMetrics()
        {
            DailyConsistencyScores = new Dictionary<DayOfWeek, double>();
            TimeBlockConsistency = new Dictionary<string, double>();
            WeekendVsWeekday = new WeekendWeekdayMetrics();
            IdentifiedPatterns = new List<ConsistencyPattern>();
            SuccessFactors = new Dictionary<string, double>();
            CommonTriggers = new List<InconsistencyTrigger>();
            StreakMetrics = new StreakConsistencyMetrics();
            RecoveryPatterns = new List<ConsistencyRecoveryPattern>();
        }
    }

    public class WeekendWeekdayMetrics
    {
        [Range(0, 100)]
        public double WeekdayConsistency { get; set; }

        [Range(0, 100)]
        public double WeekendConsistency { get; set; }

        [MaxLength(20)]
        public string PreferredDays { get; set; }  // "Weekdays", "Weekends", "Balanced"

        public Dictionary<string, double> TimeBlockComparison { get; set; }
        public List<string> WeakestPatterns { get; set; }

        public WeekendWeekdayMetrics()
        {
            TimeBlockComparison = new Dictionary<string, double>();
            WeakestPatterns = new List<string>();
        }
    }

    public class ConsistencyPattern
    {
        [MaxLength(50)]
        public string PatternType { get; set; }  // e.g., "Time-based", "Day-based", "Activity-based"

        [MaxLength(500)]
        public string Description { get; set; }

        [Range(0, 100)]
        public double SuccessRate { get; set; }

        public List<string> ContributingFactors { get; set; }
        public Dictionary<string, double> TimeDistribution { get; set; }
        public bool IsActive { get; set; }

        public ConsistencyPattern()
        {
            ContributingFactors = new List<string>();
            TimeDistribution = new Dictionary<string, double>();
        }
    }

    public class InconsistencyTrigger
    {
        [MaxLength(100)]
        public string TriggerType { get; set; }

        [Range(0, int.MaxValue)]
        public int Frequency { get; set; }

        [Range(0, 100)]
        public double ImpactScore { get; set; }  // 0-100

        public List<string> AffectedTimeBlocks { get; set; }
        public List<string> PreventiveStrategies { get; set; }
        public Dictionary<DayOfWeek, int> DayDistribution { get; set; }

        public InconsistencyTrigger()
        {
            AffectedTimeBlocks = new List<string>();
            PreventiveStrategies = new List<string>();
            DayDistribution = new Dictionary<DayOfWeek, int>();
        }
    }

    public class StreakConsistencyMetrics
    {
        [Range(0, 100)]
        public double AverageStreakConsistency { get; set; }

        [Range(0, int.MaxValue)]
        public int OptimalStreakLength { get; set; }

        public Dictionary<int, double> StreakLengthImpact { get; set; }  // Length -> Consistency
        public List<string> StreakMaintenanceFactors { get; set; }
        public Dictionary<string, double> StreakBreakdownCauses { get; set; }

        public StreakConsistencyMetrics()
        {
            StreakLengthImpact = new Dictionary<int, double>();
            StreakMaintenanceFactors = new List<string>();
            StreakBreakdownCauses = new Dictionary<string, double>();
        }
    }

    public class ConsistencyRecoveryPattern
    {
        [MaxLength(50)]
        public string PatternType { get; set; }

        [Range(0, 100)]
        public double RecoveryRate { get; set; }

        [Range(0, int.MaxValue)]
        public int AverageDaysToRecover { get; set; }

        public List<string> EffectiveStrategies { get; set; }
        public Dictionary<string, double> TimeFactors { get; set; }
        public List<string> SupportingHabits { get; set; }

        public ConsistencyRecoveryPattern()
        {
            EffectiveStrategies = new List<string>();
            TimeFactors = new Dictionary<string, double>();
            SupportingHabits = new List<string>();
        }
    }
}