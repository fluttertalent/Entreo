using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Entreo.Models.Enums;
using WebApp.Entreo.Shared.Models;

namespace WebApp.Entreo.Models
{
    public class Habit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Index(nameof(Title), IsUnique = true)]
        public string Title { get; set; }  // "Morning Meditation", "Daily Exercise", "Read 30 Minutes"

        [MaxLength(500)]
        public string Description { get; set; }  // Detailed description of the habit and its execution

        [Required]
        public HabitType Type { get; set; } // e.g., Cue, Stacked, IdentityBased, etc.

        [Required]
        public bool IsTemplate { get; set; } = false;

        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string UserId { get; set; }  // Foreign key to AspNetUsers table
        public virtual User User { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [MaxLength(24)]
        public string Source { get; set; }  // "Entreo", "User"

        // Atomic Habits' Four Laws
        [MaxLength(200)]
        public string CueDescription { get; set; }  // "When I finish my morning coffee", "After I park my car at work"

        [MaxLength(200)]
        public string CravingDescription { get; set; }  // "I want to feel energized", "I want to be more knowledgeable"

        [MaxLength(200)]
        public string ResponseDescription { get; set; }  // "I will do 10 pushups", "I will read one chapter"

        [MaxLength(200)]
        public string RewardDescription { get; set; }  // "I feel accomplished", "I learn something new"

        // Frequency
        [Required]
        public FrequencyType FrequencyType { get; set; }
        public int? FrequencyCount { get; set; }  // e.g., 3 times per FrequencyType

        [MaxLength(300)]
        public string WhyStatement { get; set; }  // Explains motivation: "Exercise keeps me healthy and energetic"

        // Implementation Intention
        [MaxLength(200)]
        public string WhenTrigger { get; set; }  // Specific time/event trigger: "After lunch break", "At 7 AM"

        [MaxLength(200)]
        public string WhereTrigger { get; set; }  // Location trigger: "At my desk", "In the home gym"

        // Environment Design
        [MaxLength(300)]
        public string EnvironmentSetup { get; set; }  // How to prepare environment: "Place running shoes by bed"

        [MaxLength(300)]
        public string ObstacleRemoval { get; set; }  // How to remove barriers: "Delete social media apps before study time"

        // 2-Minute Rule
        [MaxLength(200)]
        public string MinimalVersion { get; set; }  // Simplified version: "Put on running shoes", "Open book"

        [MaxLength(200)]
        public string FullVersion { get; set; }  // Complete version: "Run 5km", "Read 30 pages"
        // Tracking
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? LastCompletedAt { get; set; }

        public TimeSpan EstimatedDuration { get; set; }

        public bool IsArchived { get; set; }
        public decimal? TargetValue { get; set; }  // e.g. 10.0 (minutes)
        public DifficultyLevel Difficulty { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CompletionCount { get; set; }

        // Relationships
        public virtual ICollection<HabitLog> HabitLogs { get; set; }
        public virtual ICollection<HabitReminder> Reminders { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }

        // Update constructor
        public Habit()
        {
            HabitLogs = new HashSet<HabitLog>();
            Reminders = new HashSet<HabitReminder>();
            Tags = new HashSet<Tag>();
        }
    }

    public enum HabitType
    {
        Stacking,                   //This involves pairing a new habit with an existing one, using the established routine as a trigger. For instance, if you already have a habit of drinking coffee in the morning, you might stack the habit of reading for 10 minutes right afterward.
        IdentityBased,              //Instead of focusing on what you want to achieve (an outcome), you focus on who you want to become (an identity). This shifts the motivation from completing tasks to embodying traits. For example, rather than saying, "I want to run a marathon," you think, "I am a runner."
        EnvironmentDesign,          //This strategy makes it easier to stick to habits by shaping your physical surroundings. If you want to read more, you might leave a book on your bedside table. If you want to snack healthier, place fruits in a visible spot.
        ImplementationIntentions,   //This involves planning out when and where youll perform a habit by using an if-then format. For instance, "If its lunchtime, then Ill eat a salad." This approach helps integrate habits seamlessly into daily routines.
        EmptationBundling,          //Pair a habit you need to do with a habit you want to do. For example, listen to your favorite podcast only while doing household chores. This strategy combines extrinsic motivation with an enjoyable reward.
        TwoMinuteRule,              //To make habits easier to adopt, start with a version that takes two minutes or less. Instead ofrunning five kilometers, start withputting on running shoes. The idea is to make starting the habit easy and build from there.
        CueBased                    //These are habits that are triggered by specific cues in your environment, like washing your hands before eating. The cue prompts the action, reinforcing the habit automatically.
    }

    public enum DifficultyLevel
    {
        Undefined = 0,
        VeryEasy = 1,
        Easy = 2,
        Moderate = 3,
        Hard = 4,
        VeryHard = 5
    }
}