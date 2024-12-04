using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Models
{
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }  // e.g., "Health", "Productivity", "Learning"

        [Required]
        [MaxLength(16)]
        public TagCategoryType Category { get; set; }  // e.g., "Sport", "Diet", "Relaxation"

        public int Order { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual ICollection<Habit> Habits { get; set; }

        public Tag()
        {
            Habits = new HashSet<Habit>();
        }
    }

    public enum TagCategoryType
    {
        Sports,
        Diet,
        Relaxation,
        //...
    }

}