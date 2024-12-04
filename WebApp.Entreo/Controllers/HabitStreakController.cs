using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Entreo.Infrastructure.Auth;
using WebApp.Entreo.Services;

namespace WebApp.Entreo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HabitStreakController : ControllerBase
    {
        private readonly HabitStreakService _streakService;
        private readonly ILogger<HabitStreakController> _logger;
        private readonly IUserAccessor _userAccessor;

        public HabitStreakController(
            HabitStreakService streakService,
            ILogger<HabitStreakController> logger,
            IUserAccessor userAccessor)
        {
            _streakService = streakService;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        [HttpGet("habit/{habitId}")]
        public async Task<int> GetCurrentStreak(int habitId)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _streakService.CalculateCurrentStreak(habitId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get current streak for habit {HabitId}, user {UserId}",
                    habitId, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("habit/{habitId}/longest")]
        public async Task<int> GetLongestStreak(int habitId)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _streakService.GetLongestStreak(habitId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get longest streak for habit {HabitId}, user {UserId}",
                    habitId, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("user/current")]
        public async Task<Dictionary<int, int>> GetAllCurrentStreaks()
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _streakService.GetAllCurrentStreaks(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all current streaks for user {UserId}",
                    _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("user/longest")]
        public async Task<Dictionary<int, int>> GetAllLongestStreaks()
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _streakService.GetAllLongestStreaks(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all longest streaks for user {UserId}",
                    _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("user/stats")]
        public async Task<UserStreakStats> GetUserStreakStats()
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _streakService.GetUserStreakStats(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak stats for user {UserId}",
                    _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("habit/{habitId}/history")]
        public async Task<List<StreakHistory>> GetStreakHistory(int habitId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _streakService.GetStreakHistory(habitId, userId, from, to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak history for habit {HabitId}, user {UserId}",
                    habitId, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("habit/{habitId}/calendar")]
        public async Task<Dictionary<DateTime, bool>> GetStreakCalendar(int habitId, [FromQuery] DateTime month)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _streakService.GetStreakCalendar(habitId, userId, month);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak calendar for habit {HabitId}, user {UserId}, month {Month}",
                    habitId, _userAccessor.GetCurrentUserId(), month.ToString("yyyy-MM"));
                throw;
            }
        }

        [HttpGet("user/overview")]
        public async Task<StreakOverview> GetUserStreakOverview([FromQuery] DateTime? date)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _streakService.GetUserStreakOverview(userId, date ?? DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak overview for user {UserId}",
                    _userAccessor.GetCurrentUserId());
                throw;
            }
        }
    }

    public class UserStreakStats
    {
        public int TotalHabits { get; set; }
        public int HabitsWithStreak { get; set; }
        public int LongestEverStreak { get; set; }
        public int CurrentLongestStreak { get; set; }
        public double AverageStreak { get; set; }
        public Dictionary<int, int> StreakDistribution { get; set; }
    }

    public class StreakHistory
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Length { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class StreakOverview
    {
        public int TotalActiveStreaks { get; set; }
        public int TotalStreakDays { get; set; }
        public Dictionary<int, int> CurrentStreaks { get; set; }
        public Dictionary<int, DateTime> LastCompletions { get; set; }
        public List<int> HabitsAtRisk { get; set; }
        public Dictionary<int, int> StreakChanges { get; set; }
    }
}