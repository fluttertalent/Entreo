using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int HabitId { get; set; }

        [Required]
        public DateTime CompletedAt { get; set; }

        [MaxLength(500)]
        public string Note { get; set; }  // Optional note about the completion

        public int StreakCount { get; set; }
        public bool WasSkipped { get; set; }

        // Navigation property
        public virtual Habit Habit { get; set; }
    }
}