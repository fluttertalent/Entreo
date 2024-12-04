using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitProgress
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int HabitId { get; set; }
        public virtual Habit Habit { get; set; }

        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }

        // Basic Tracking
        public DateTime Date { get; set; }
        public bool IsCompleted { get; set; }
        public decimal? MeasurementValue { get; set; }  // e.g., minutes meditated, pages read
        [MaxLength(500)]
        public string Notes { get; set; }

        // Quality Metrics
        public int? Quality { get; set; }  // 1-5 rating of how well it was done
        public int? Difficulty { get; set; }  // 1-5 rating of how hard it felt
        public int? Satisfaction { get; set; }  // 1-5 rating of satisfaction

        // Time Tracking
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? Duration { get; set; }

        // Context
        [MaxLength(100)]
        public string Location { get; set; }  // Where it was done
        [MaxLength(50)]
        public string Mood { get; set; }      // How you felt
        [MaxLength(20)]
        public string Energy { get; set; }    // Energy level (High/Medium/Low)

        // Obstacles & Enablers
        [MaxLength(500)]
        public string Obstacles { get; set; }  // What made it difficult
        [MaxLength(500)]
        public string Enablers { get; set; }   // What made it easier

        // Progress Indicators
        public decimal? ProgressTowardsGoal { get; set; }  // Percentage
        public bool ExceededTarget { get; set; }
        [MaxLength(500)]
        public string ProgressNotes { get; set; }
    }
}