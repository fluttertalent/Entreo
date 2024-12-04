using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Shared.Models
{
    public class User
    {
        [Key]
        [Required]
        [StringLength(450)]  // Max length for ASP.NET Identity UserId
        public string Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email address for this user.
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(256)]
        [ProtectedPersonalData]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        [StringLength(100)]
        public virtual string? PasswordHash { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        [StringLength(100)]
        public string? SecurityStamp { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        // Navigation properties
        public virtual ICollection<Habit> Habits { get; set; }
        public virtual ICollection<HabitLog> HabitLogs { get; set; }
        public virtual ICollection<HabitReminder> HabitReminders { get; set; }
        public virtual ICollection<UserNotificationPreference> NotificationPreferences { get; set; }

        public User()
        {
            Habits = new HashSet<Habit>();
            HabitLogs = new HashSet<HabitLog>();
            HabitReminders = new HashSet<HabitReminder>();
            NotificationPreferences = new HashSet<UserNotificationPreference>();
        }
    }
}