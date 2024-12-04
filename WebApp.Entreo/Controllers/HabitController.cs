using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApp.Entreo.Attributes;
using WebApp.Entreo.Exceptions;
using WebApp.Entreo.Infrastructure.Auth;
using WebApp.Entreo.Models;
using WebApp.Entreo.Services;
using WebApp.Entreo.Shared.Models;

namespace WebApp.Entreo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class HabitController : ControllerBase
    {
        private readonly HabitService _habitService;
        private readonly ILogger<HabitController> _logger;
        private readonly IUserAccessor _userAccessor;

        public HabitController(
            HabitService habitService,
            ILogger<HabitController> logger,
            IUserAccessor userAccessor)
        {
            _habitService = habitService;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        [HttpGet]
        public async Task<List<Habit>> GetHabits()
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.GetUserHabits(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habits for user {UserId}", _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<Habit> GetHabit(int id)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                var habit = await _habitService.GetHabit(id, userId);

                if (habit == null)
                {
                    _logger.LogWarning("Habit {HabitId} not found for user {UserId}", id, userId);
                    throw new NotFoundException($"Habit {id} not found");
                }

                return habit;
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Failed to get habit {HabitId} for user {UserId}",
                    id, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<Habit> CreateHabit([FromBody] Habit habit)
        {
            try
            {
                habit.UserId = _userAccessor.GetCurrentUserId();
                return await _habitService.CreateHabit(habit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create habit for user {UserId}",
                    _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<HabitLog> LogCompletion(int id, [FromBody] HabitLog log)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.LogCompletion(id, userId, log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log completion for habit {HabitId}, user {UserId}",
                    id, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<Habit> UpdateHabit(int id, [FromBody] Habit habit)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();

                if (habit.Id != id || habit.UserId != userId)
                {
                    _logger.LogWarning("Invalid habit update attempt: ID mismatch or wrong user");
                    throw new ValidationException("Invalid habit data");
                }

                var existingHabit = await _habitService.GetHabit(id, userId);
                if (existingHabit == null)
                {
                    _logger.LogWarning("Update attempted on non-existent habit {HabitId}", id);
                    throw new NotFoundException($"Habit {id} not found");
                }

                return await _habitService.UpdateHabit(habit);
            }
            catch (Exception ex) when (!(ex is NotFoundException || ex is ValidationException))
            {
                _logger.LogError(ex, "Failed to update habit {HabitId} for user {UserId}",
                    id, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteHabit(int id)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                var habit = await _habitService.GetHabit(id, userId);

                if (habit == null)
                {
                    _logger.LogWarning("Delete attempted on non-existent habit {HabitId}", id);
                    throw new NotFoundException($"Habit {id} not found");
                }

                await _habitService.DeleteHabit(id);
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Failed to delete habit {HabitId} for user {UserId}",
                    id, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("{id}/streak")]
        public async Task<HabitStreak> GetHabitStreak(int id)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                var habit = await _habitService.GetHabit(id, userId);

                if (habit == null)
                {
                    _logger.LogWarning("Streak request for non-existent habit {HabitId}", id);
                    throw new NotFoundException($"Habit {id} not found");
                }

                return new HabitStreak
                {
                    HabitId = id,
                    CurrentStreak = habit.CurrentStreak,
                    LastCompletedAt = habit.LastCompletedAt
                };
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Failed to get streak for habit {HabitId}, user {UserId}",
                    id, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("stats")]
        public async Task<HabitStats> GetHabitStats()
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                var habits = await _habitService.GetUserHabits(userId);

                return new HabitStats
                {
                    TotalHabits = habits.Count,
                    ActiveHabits = habits.Count(h => !h.IsArchived),
                    ArchivedHabits = habits.Count(h => h.IsArchived),
                    //HabitsWithStreak = habits.Count(h => h.CurrentStreak > 0),
                    //TotalCompletions = habits.Sum(h => h.CompletionCount),
                    //AverageStreak = habits.Any() ? habits.Average(h => h.CurrentStreak) : 0,
                    //LongestStreak = habits.Any() ? habits.Max(h => h.CurrentStreak) : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habit stats for user {UserId}",
                    _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("calendar")]
        public async Task<Dictionary<DateTime, List<HabitCompletion>>> GetHabitCalendar(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.GetHabitCalendar(userId, from, to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habit calendar for user {UserId}", _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("{id}/history")]
        public async Task<List<HabitHistory>> GetHabitHistory(
            int id,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.GetHabitHistory(id, userId, from, to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get history for habit {HabitId}", id);
                throw;
            }
        }

        [HttpPost("{id}/archive")]
        public async Task<Habit> ArchiveHabit(int id)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.ArchiveHabit(id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to archive habit {HabitId}", id);
                throw;
            }
        }

        [HttpPost("{id}/unarchive")]
        public async Task<Habit> UnarchiveHabit(int id)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.UnarchiveHabit(id, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unarchive habit {HabitId}", id);
                throw;
            }
        }

        [HttpGet("suggestions")]
        public async Task<List<HabitSuggestion>> GetHabitSuggestions()
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.GetHabitSuggestions(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habit suggestions for user {UserId}", _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("{id}/analytics")]
        public async Task<HabitAnalytics> GetHabitAnalytics(
            int id,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.GetHabitAnalytics(userId, id, from, to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get analytics for habit {HabitId}", id);
                throw;
            }
        }

        [HttpPost("{id}/reminder")]
        public async Task<HabitReminder> SetHabitReminder(int id, [FromBody] HabitReminder reminder)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.SetHabitReminder(id, userId, reminder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set reminder for habit {HabitId}", id);
                throw;
            }
        }

        [HttpGet("report")]
        public async Task<HabitReport> GetHabitReport(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string format = "summary")
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.GenerateHabitReport(userId, from, to, format);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate habit report for user {UserId}", _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("search")]
        public async Task<List<Habit>> SearchHabits([FromQuery] HabitSearchParams searchParams)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _habitService.SearchHabits(userId, searchParams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search habits for user {UserId}", _userAccessor.GetCurrentUserId());
                throw;
            }
        }
    }

    public class HabitStreak
    {
        public int HabitId { get; set; }
        public int CurrentStreak { get; set; }
        public DateTime? LastCompletedAt { get; set; }
    }

    public class HabitStats
    {
        public int TotalHabits { get; set; }
        public int ActiveHabits { get; set; }
        public int ArchivedHabits { get; set; }
        public int HabitsWithStreak { get; set; }
        public int TotalCompletions { get; set; }
        public double AverageStreak { get; set; }
        public int LongestStreak { get; set; }
    }
}