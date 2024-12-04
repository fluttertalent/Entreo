using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Entreo.Shared.Models;

namespace WebApp.Entreo.Models
{
    public class HabitLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        public int HabitId { get; set; }
        public virtual Habit Habit { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime CompletedAt { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }  // Optional notes about the log entry
                                           // Optional measurement of the habit (e.g., minutes exercised, pages read)
        public decimal? Measurement { get; set; }

        [StringLength(20)]
        public string MeasurementUnit { get; set; }  // e.g., "minutes", "pages", "kilometers"

        public TimeSpan? Duration { get; set; }
    }
}