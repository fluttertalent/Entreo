using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitResource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int HabitId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        [Url]
        public string Url { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }

        public virtual Habit Habit { get; set; }
    }

    public enum ResourceType
    {
        Article,
        Video,
        Book,
        Podcast,
        Course,
        Tool,
        Community
    }
}