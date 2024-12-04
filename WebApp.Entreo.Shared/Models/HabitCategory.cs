using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Models
{
    public class HabitCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Index(IsUnique = true)]
        public string Name { get; set; }  // e.g., Health, Finance, Learning

        [MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(7)]
        public string ColorHex { get; set; }  // e.g., "#FF0000"

        [MaxLength(50)]
        public string Icon { get; set; }  // e.g., "fitness", "book", "money"

        public virtual ICollection<Habit> Habits { get; set; }
    }
}