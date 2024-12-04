using System.ComponentModel.DataAnnotations;

namespace WebApp.Entreo.Shared.Models
{
    public class HabitSearchParams
    {
        // Text search
        [MaxLength(100)]
        public string SearchTerm { get; set; }

        // Filters
        [MaxLength(50)]
        public string Category { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsActive { get; set; }
        public int? MinStreak { get; set; }
        public int? MaxStreak { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public DateTime? LastCompletedFrom { get; set; }
        public DateTime? LastCompletedTo { get; set; }

        // Sorting
        [MaxLength(50)]
        public string SortBy { get; set; } = "CreatedAt"; // Default sort field
        public bool SortDescending { get; set; } = true;   // Default sort direction

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Additional filters
        [MaxLength(50)]
        public List<string> Tags { get; set; }
        public int? MinCompletions { get; set; }
        public double? MinCompletionRate { get; set; }

        public HabitSearchParams()
        {
            Tags = new List<string>();
        }

        // Helper method to validate and adjust pagination
        public void ValidatePagination()
        {
            Page = Math.Max(1, Page);
            PageSize = Math.Clamp(PageSize, 1, 100);
        }

        // Helper method to get valid sort fields
        public static readonly HashSet<string> ValidSortFields = new()
        {
            "CreatedAt",
            "Title",
            "Category",
            "CurrentStreak",
            "LastCompletedAt",
            "CompletionCount"
        };

        // Helper method to validate sort field
        public void ValidateSort()
        {
            if (string.IsNullOrEmpty(SortBy) || !ValidSortFields.Contains(SortBy))
            {
                SortBy = "CreatedAt";
            }
        }
    }
}