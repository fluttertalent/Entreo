namespace WebApp.Entreo.Models.Filters
{
    public class HabitLogFilter : BaseFilter
    {
        public int? HabitId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinMeasurement { get; set; }
        public decimal? MaxMeasurement { get; set; }
    }

    public class BaseFilter
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int Page { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Min(value, MaxPageSize);
        }

        public string SortBy { get; set; } = "Id";
        public bool SortDescending { get; set; } = false;
    }
}