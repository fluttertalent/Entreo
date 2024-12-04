using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitCompletion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int HabitId { get; set; }

        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string HabitTitle { get; set; }  // Title of the completed habit

        [Required]
        public DateTime CompletedAt { get; set; }

        [MaxLength(500)]
        public string Note { get; set; }  // Optional completion notes

        public bool IsSkipped { get; set; }

        [ForeignKey("HabitId")]
        public virtual Habit Habit { get; set; }
    }
}