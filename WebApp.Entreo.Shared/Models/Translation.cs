using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

[Index(nameof(LanguageCode), nameof(FieldName), IsUnique = true)]
public class Translation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string LanguageCode { get; set; }  // e.g., "en", "es", "fr"

    [Required]
    [MaxLength(100)]
    public string FieldName { get; set; }  // e.g., "Title", "Description"

    [Required]
    [MaxLength(1000)]  // Assuming a reasonable max length for translated content
    public string Value { get; set; }  // The actual translated text

    [MaxLength(50)]
    public string TranslationSource { get; set; }  // Manual, AI, External

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Translation()
    {
        CreatedAt = DateTime.UtcNow;
    }
}