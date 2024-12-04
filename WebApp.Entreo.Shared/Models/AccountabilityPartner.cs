using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Models
{
    /// <summary>
    /// Represents a user's accountability partner relationship for habit tracking and mutual support
    /// </summary>
    public class AccountabilityPartner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        [ForeignKey("User")]
        public string UserId { get; set; }  // The user who initiated the partnership

        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        [ForeignKey("Partner")]
        public string PartnerId { get; set; }  // The accountability partner's user ID

        [Required]
        public DateTime ConnectedDate { get; set; }  // When the partnership was established

        [Required]
        public bool CanViewAll { get; set; }  // Determines if partner can view all habits or only shared ones

        public virtual ICollection<Habit> SharedHabits { get; set; }  // Collection of habits shared with this partner
    }
}