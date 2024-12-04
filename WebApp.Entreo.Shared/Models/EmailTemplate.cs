using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace WebApp.Entreo.Shared.Models
{
    /// <summary>
    /// Represents an email template for system notifications and communications
    /// </summary>
    [Index(nameof(Name), IsUnique = true)]
    public class EmailTemplate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }  // e.g., "WelcomeEmail", "HabitReminderEmail"

        [Required]
        [MaxLength(10000)]  // Adjust max length based on your needs
        public string HtmlContent { get; set; }  // HTML version of the email content

        [MaxLength(5000)]  // Adjust max length based on your needs
        public string PlainTextContent { get; set; }  // Plain text version of the email content

        [MaxLength(200)]
        public string Description { get; set; }  // Brief description of when and how this template is used

        public bool IsActive { get; set; }
    }
}