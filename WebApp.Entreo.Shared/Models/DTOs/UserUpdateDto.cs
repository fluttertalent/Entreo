using System.ComponentModel.DataAnnotations;

namespace WebApp.Entreo.Shared.Models.DTOs
{
    public class UserUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}