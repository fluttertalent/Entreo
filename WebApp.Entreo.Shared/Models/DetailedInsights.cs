using System.ComponentModel.DataAnnotations;

namespace WebApp.Entreo.Shared.Models
{
    public class DetailedInsights
    {
        public List<HabitInsight> HabitInsights { get; set; }
        public TimePatternInsights TimePatterns { get; set; }
        public SuccessPatternInsights SuccessPatterns { get; set; }

        [MaxLength(500)]
        public List<string> Suggestions { get; set; }  // List of actionable improvement suggestions

        public DetailedInsights()
        {
            HabitInsights = new List<HabitInsight>();
            Suggestions = new List<string>();
        }
    }

    public class HabitInsight
    {
        public int HabitId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }  // Name of the habit being analyzed

        public int CompletionCount { get; set; }
        public int SkipCount { get; set; }
        public double SuccessRate { get; set; }
        public List<DayOfWeek> BestPerformingDays { get; set; }
        public double AverageCompletionsPerWeek { get; set; }
    }

    public class TimePatternInsights
    {
        [Required]
        [MaxLength(20)]
        public string MostProductiveTimeOfDay { get; set; }  // "Morning", "Afternoon", "Evening", "Night"

        public Dictionary<DayOfWeek, int> DayOfWeekDistribution { get; set; }
        public WeekendWeekdayComparison WeekendVsWeekday { get; set; }

        [MaxLength(100)]
        public List<string> ConsistentTimeSlots { get; set; }  // e.g., "8:00 AM", "After lunch"

        public TimePatternInsights()
        {
            DayOfWeekDistribution = new Dictionary<DayOfWeek, int>();
            ConsistentTimeSlots = new List<string>();
        }
    }

    public class WeekendWeekdayComparison
    {
        public double WeekdayAverage { get; set; }
        public double WeekendAverage { get; set; }
        public double Difference { get; set; }

        [MaxLength(200)]
        public string Recommendation { get; set; }  // Suggested adjustments based on weekend/weekday performance
    }

    public class SuccessPatternInsights
    {
        public List<ConsecutiveDayPattern> ConsecutiveDayPatterns { get; set; }

        [MaxLength(200)]
        public List<string> SuccessfulSequences { get; set; }  // Patterns that led to successful habit completion

        [MaxLength(200)]
        public List<string> FailurePatterns { get; set; }  // Common patterns leading to missed habits

        public RecoveryAnalysis RecoveryPatterns { get; set; }

        public SuccessPatternInsights()
        {
            ConsecutiveDayPatterns = new List<ConsecutiveDayPattern>();
            SuccessfulSequences = new List<string>();
            FailurePatterns = new List<string>();
        }
    }

    public class ConsecutiveDayPattern
    {
        public int Length { get; set; }
        public int Frequency { get; set; }
        public DateTime? MostRecentOccurrence { get; set; }

        [Required]
        [MaxLength(100)]
        public string Pattern { get; set; }  // e.g., "Monday-Wednesday-Friday"
    }

    public class RecoveryAnalysis
    {
        public double AverageRecoveryTime { get; set; }  // Days to resume after break

        [MaxLength(200)]
        public List<string> SuccessfulRecoveryStrategies { get; set; }  // Effective methods for getting back on track

        public double RecoverySuccessRate { get; set; }

        [MaxLength(200)]
        public string MostEffectiveComeback { get; set; }  // Best strategy for habit recovery

        public RecoveryAnalysis()
        {
            SuccessfulRecoveryStrategies = new List<string>();
        }
    }
}