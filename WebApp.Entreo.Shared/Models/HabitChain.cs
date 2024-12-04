using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Models
{
    public class HabitChain
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public int EstimatedDurationMinutes { get; set; }

        // Ordered list of habits that should be performed in sequence
        public virtual ICollection<HabitChainItem> ChainItems { get; set; }
    }

    public class HabitChainItem
    {
        public int Id { get; set; }

        [Required]
        public int HabitChainId { get; set; }
        public virtual HabitChain HabitChain { get; set; }

        [Required]
        public int HabitId { get; set; }
        public virtual Habit Habit { get; set; }

        [Required]
        public int OrderInChain { get; set; }

        public int? DurationMinutes { get; set; }
        public bool IsRequired { get; set; }
    }
}