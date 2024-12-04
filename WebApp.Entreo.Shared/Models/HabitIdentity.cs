using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Entreo.Models;

public class HabitIdentity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int HabitId { get; set; }

    [Required]
    [MaxLength(200)]
    public string IdentityStatement { get; set; }  // e.g. "I am a reader"

    [MaxLength(500)]
    public string WhyStatement { get; set; }  // e.g. "Reading makes me more knowledgeable"

    [Range(1, 5)]
    public int IdentityAlignment { get; set; }  // 1-5 scale

    [MaxLength(500)]
    public string IdentityVision { get; set; }  // Long-term identity vision

    public virtual Habit Habit { get; set; }
}