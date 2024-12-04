using System.ComponentModel.DataAnnotations;

namespace WebApp.Entreo.Shared.Models.DTOs
{
    public class UserDto
    {
        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public bool IsAuthenticated { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        // Computed properties
        public string FullName => $"{FirstName} {LastName}";

        public bool HasPhoneNumber => !string.IsNullOrEmpty(PhoneNumber);

        public bool IsEmailConfirmed { get; set; }

        public bool IsPhoneNumberConfirmed { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string TimeZone { get; set; }

        public string Language { get; set; }
    }
}