namespace WebApp.Entreo.Shared.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class HabitSuggestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        [MaxLength(20)]
        public string DifficultyLevel { get; set; }  // Beginner, Intermediate, Advanced

        public int EstimatedTimeMinutes { get; set; }

        public List<string> Tags { get; set; }

        [MaxLength(500)]
        public string Benefits { get; set; }

        [MaxLength(50)]
        public string RecommendedFrequency { get; set; }  // e.g., "Daily", "Weekly", "3x/week"

        [MaxLength(500)]
        public string GettingStartedTips { get; set; }
    }
}