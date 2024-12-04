using Microsoft.EntityFrameworkCore;
using WebApp.Entreo.Controllers;
using WebApp.Entreo.Data;

namespace WebApp.Entreo.Services
{
    public class HabitStreakService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HabitStreakService> _logger;
        private const string CACHE_GROUP = "HabitStreaks";

        public HabitStreakService(
            ApplicationDbContext context,
            ILogger<HabitStreakService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> CalculateCurrentStreak(int habitId, string userId)
        {
            try
            {
                var logs = await _context.HabitLogs
                    .Where(l => l.HabitId == habitId && l.UserId == userId)
                    .OrderByDescending(l => l.CompletedAt)
                    .ToListAsync();

                if (!logs.Any()) return 0;

                var streak = 1;
                var lastDate = logs[0].CompletedAt.Date;

                for (int i = 1; i < logs.Count; i++)
                {
                    var currentDate = logs[i].CompletedAt.Date;
                    if ((lastDate - currentDate).Days == 1)
                    {
                        streak++;
                        lastDate = currentDate;
                    }
                    else break;
                }

                return streak;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate current streak for habit {HabitId}", habitId);
                throw;
            }
        }

        public async Task<int> GetLongestStreak(int habitId, string userId)
        {
            try
            {
                var logs = await _context.HabitLogs
                    .Where(l => l.HabitId == habitId && l.UserId == userId)
                    .OrderBy(l => l.CompletedAt)
                    .ToListAsync();

                if (!logs.Any()) return 0;

                var longestStreak = 1;
                var currentStreak = 1;
                var lastDate = logs[0].CompletedAt.Date;

                for (int i = 1; i < logs.Count; i++)
                {
                    var currentDate = logs[i].CompletedAt.Date;
                    if ((currentDate - lastDate).Days == 1)
                    {
                        currentStreak++;
                        longestStreak = Math.Max(longestStreak, currentStreak);
                    }
                    else
                    {
                        currentStreak = 1;
                    }
                    lastDate = currentDate;
                }

                return longestStreak;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get longest streak for habit {HabitId}", habitId);
                throw;
            }
        }

        public async Task<Dictionary<int, int>> GetAllCurrentStreaks(string userId)
        {
            try
            {
                var habits = await _context.Habits
                    .Where(h => h.UserId == userId && !h.IsArchived)
                    .Select(h => h.Id)
                    .ToListAsync();

                var result = new Dictionary<int, int>();
                foreach (var habitId in habits)
                {
                    result[habitId] = await CalculateCurrentStreak(habitId, userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all current streaks for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Dictionary<int, int>> GetAllLongestStreaks(string userId)
        {
            try
            {
                var habits = await _context.Habits
                    .Where(h => h.UserId == userId)
                    .Select(h => h.Id)
                    .ToListAsync();

                var result = new Dictionary<int, int>();
                foreach (var habitId in habits)
                {
                    result[habitId] = await GetLongestStreak(habitId, userId);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all longest streaks for user {UserId}", userId);
                throw;
            }
        }

        public async Task<UserStreakStats> GetUserStreakStats(string userId)
        {
            try
            {
                var habits = await _context.Habits
                    .Where(h => h.UserId == userId && !h.IsArchived)
                    .ToListAsync();

                var currentStreaks = await GetAllCurrentStreaks(userId);

                var stats = new UserStreakStats
                {
                    TotalHabits = habits.Count,
                    HabitsWithStreak = currentStreaks.Count(s => s.Value > 0),
                    LongestEverStreak = 0,
                    CurrentLongestStreak = currentStreaks.Values.DefaultIfEmpty(0).Max(),
                    AverageStreak = currentStreaks.Values.DefaultIfEmpty(0).Average(),
                    StreakDistribution = currentStreaks
                        .GroupBy(s => s.Value)
                        .ToDictionary(g => g.Key, g => g.Count())
                };

                foreach (var habitId in habits.Select(h => h.Id))
                {
                    var longest = await GetLongestStreak(habitId, userId);
                    stats.LongestEverStreak = Math.Max(stats.LongestEverStreak, longest);
                }

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak stats for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<StreakHistory>> GetStreakHistory(int habitId, string userId, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var logs = await _context.HabitLogs
                    .Where(l => l.HabitId == habitId && l.UserId == userId)
                    .OrderBy(l => l.CompletedAt)
                    .ToListAsync();

                var streaks = new List<StreakHistory>();
                if (!logs.Any()) return streaks;

                var startDate = logs[0].CompletedAt.Date;
                var currentStreak = 1;

                for (int i = 1; i < logs.Count; i++)
                {
                    var currentDate = logs[i].CompletedAt.Date;
                    if ((currentDate - logs[i - 1].CompletedAt.Date).Days == 1)
                    {
                        currentStreak++;
                    }
                    else
                    {
                        if (currentStreak > 1)
                        {
                            streaks.Add(new StreakHistory
                            {
                                StartDate = startDate,
                                EndDate = logs[i - 1].CompletedAt.Date,
                                Length = currentStreak,
                                IsCurrent = false
                            });
                        }
                        startDate = currentDate;
                        currentStreak = 1;
                    }
                }

                // Add the last/current streak
                if (currentStreak > 0)
                {
                    streaks.Add(new StreakHistory
                    {
                        StartDate = startDate,
                        EndDate = logs.Last().CompletedAt.Date,
                        Length = currentStreak,
                        IsCurrent = (logs.Last().CompletedAt.Date - DateTime.UtcNow.Date).Days >= -1
                    });
                }

                if (from.HasValue)
                    streaks = streaks.Where(s => s.EndDate >= from.Value).ToList();
                if (to.HasValue)
                    streaks = streaks.Where(s => s.StartDate <= to.Value).ToList();

                return streaks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak history for habit {HabitId}", habitId);
                throw;
            }
        }

        public async Task<Dictionary<DateTime, bool>> GetStreakCalendar(int habitId, string userId, DateTime month)
        {
            try
            {
                var startDate = new DateTime(month.Year, month.Month, 1);
                var endDate = startDate.AddMonths(1);

                var logs = await _context.HabitLogs
                    .Where(l => l.HabitId == habitId
                        && l.UserId == userId
                        && l.CompletedAt >= startDate
                        && l.CompletedAt < endDate)
                    .Select(l => l.CompletedAt.Date)
                    .Distinct()
                    .ToListAsync();

                var calendar = new Dictionary<DateTime, bool>();
                for (var date = startDate; date < endDate; date = date.AddDays(1))
                {
                    calendar[date] = logs.Contains(date);
                }

                return calendar;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak calendar for habit {HabitId}, month {Month}",
                    habitId, month.ToString("yyyy-MM"));
                throw;
            }
        }

        public async Task<StreakOverview> GetUserStreakOverview(string userId, DateTime date)
        {
            try
            {
                var currentStreaks = await GetAllCurrentStreaks(userId);
                var habits = await _context.Habits
                    .Where(h => h.UserId == userId && !h.IsArchived)
                    .ToDictionaryAsync(h => h.Id, h => h.LastCompletedAt);

                var overview = new StreakOverview
                {
                    TotalActiveStreaks = currentStreaks.Count(s => s.Value > 0),
                    TotalStreakDays = currentStreaks.Values.Sum(),
                    CurrentStreaks = currentStreaks,
                    LastCompletions = habits.ToDictionary(h => h.Key, h => h.Value ?? DateTime.MinValue),
                    HabitsAtRisk = habits
                        .Where(h => h.Value.HasValue && (date - h.Value.Value).Days == 1)
                        .Select(h => h.Key)
                        .ToList(),
                    StreakChanges = new Dictionary<int, int>()
                };

                // Calculate streak changes
                foreach (var habit in habits)
                {
                    var previousStreak = await CalculateStreakAtDate(habit.Key, userId, date.AddDays(-1));
                    var currentStreak = currentStreaks[habit.Key];
                    if (previousStreak != currentStreak)
                    {
                        overview.StreakChanges[habit.Key] = currentStreak - previousStreak;
                    }
                }

                return overview;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get streak overview for user {UserId}", userId);
                throw;
            }
        }

        private async Task<int> CalculateStreakAtDate(int habitId, string userId, DateTime date)
        {
            try
            {
                var logs = await _context.HabitLogs
                    .Where(l => l.HabitId == habitId
                        && l.UserId == userId
                        && l.CompletedAt.Date <= date)
                    .OrderByDescending(l => l.CompletedAt)
                    .ToListAsync();

                if (!logs.Any()) return 0;

                var streak = 1;
                var lastDate = logs[0].CompletedAt.Date;

                for (int i = 1; i < logs.Count; i++)
                {
                    var currentDate = logs[i].CompletedAt.Date;
                    if ((lastDate - currentDate).Days == 1)
                    {
                        streak++;
                        lastDate = currentDate;
                    }
                    else break;
                }

                return streak;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate streak at date for habit {HabitId}", habitId);
                throw;
            }
        }
    }
}