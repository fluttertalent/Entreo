using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entreo.Models
{
    public class HabitGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? MaxParticipants { get; set; }
        public bool IsPrivate { get; set; }

        [Required]
        [StringLength(450)]
        public string CreatorUserId { get; set; }

        public virtual ICollection<HabitGroupMember> Members { get; set; }
        public virtual ICollection<HabitGroupChallenge> Challenges { get; set; }
    }

    public class HabitGroupMember
    {
        public int Id { get; set; }

        [Required]
        public int HabitGroupId { get; set; }
        public virtual HabitGroup HabitGroup { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime JoinedAt { get; set; }
        public GroupMemberRole Role { get; set; }
        public int Score { get; set; }
    }

    public class HabitGroupChallenge
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(200)]
        public string Prize { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int? TargetValue { get; set; }
    }

    public enum GroupMemberRole
    {
        Member,
        Moderator,
        Admin
    }
}