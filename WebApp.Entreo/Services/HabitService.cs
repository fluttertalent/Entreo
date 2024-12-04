using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using WebApp.Entreo.Data;
using WebApp.Entreo.Exceptions;
using WebApp.Entreo.Models;
using WebApp.Entreo.Shared.Models;

namespace WebApp.Entreo.Services
{
    public class HabitService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HabitService> _logger;
        private readonly IMemoryCache _cache;
        private const string CACHE_GROUP = "Habits";
        private const int MAX_ACTIVE_HABITS = 10; // Configurable limit

        public HabitService(
            ApplicationDbContext context,
            ILogger<HabitService> logger,
            IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        private async Task ValidateHabit(Habit habit)
        {
            if (habit == null)
                throw new ArgumentNullException(nameof(habit));

            if (string.IsNullOrWhiteSpace(habit.Title))
                throw new ValidationException("Habit title is required");

            if (habit.Title.Length > 100)
                throw new ValidationException("Habit title cannot exceed 100 characters");

            if (!string.IsNullOrEmpty(habit.Description) && habit.Description.Length > 500)
                throw new ValidationException("Habit description cannot exceed 500 characters");

            if (string.IsNullOrEmpty(habit.UserId))
                throw new ValidationException("UserId is required");

            // Check for duplicate titles for the same user
            var duplicateExists = await _context.Habits
                .AnyAsync(h => h.UserId == habit.UserId
                    && h.Title == habit.Title
                    && h.Id != habit.Id);

            if (duplicateExists)
                throw new ValidationException($"A habit with title '{habit.Title}' already exists");
        }

        private async Task ValidateHabitLimit(string userId)
        {
            var activeHabitsCount = await _context.Habits
                .CountAsync(h => h.UserId == userId && !h.IsArchived);

            if (activeHabitsCount >= MAX_ACTIVE_HABITS)
                throw new ValidationException($"User cannot have more than {MAX_ACTIVE_HABITS} active habits");
        }

        private void ValidateCompletionLog(HabitLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            if (log.CompletedAt > DateTime.UtcNow)
                throw new ValidationException("Completion date cannot be in the future");

            if (log.CompletedAt < DateTime.UtcNow.AddYears(-1))
                throw new ValidationException("Completion date cannot be more than 1 year in the past");
        }

        public async Task<List<Habit>> GetUserHabits(string userId)
        {
            try
            {
                return await _context.Habits
                    .Where(h => h.UserId == userId)
                    .OrderByDescending(h => h.LastCompletedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habits for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Habit> GetHabit(int id, string userId)
        {
            try
            {
                return await _context.Habits
                    .FirstAsync(h => h.Id == id && h.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habit {HabitId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<Habit> CreateHabit(Habit habit)
        {
            try
            {
                await ValidateHabit(habit);
                await ValidateHabitLimit(habit.UserId);

                habit.CreatedAt = DateTime.UtcNow;
                habit.UpdatedAt = DateTime.UtcNow;
                //habit.CurrentStreak = 0;
                habit.IsArchived = false;

                _context.Habits.Add(habit);
                await _context.SaveChangesAsync();

                return habit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create habit for user {UserId}", habit.UserId);
                throw;
            }
        }

        public async Task<HabitLog> LogCompletion(int habitId, string userId, HabitLog log)
        {
            try
            {
                var habit = await GetHabit(habitId, userId);
                if (habit == null)
                    throw new NotFoundException($"Habit {habitId} not found");

                ValidateCompletionLog(log);

                // Check for duplicate logs
                var duplicateExists = await _context.HabitLogs
                    .AnyAsync(l => l.HabitId == habitId
                        && l.UserId == userId
                        && l.CompletedAt.Date == log.CompletedAt.Date);

                if (duplicateExists)
                    throw new ValidationException("Habit already logged for this date");

                log.HabitId = habitId;
                log.UserId = userId;
                log.CreatedAt = DateTime.UtcNow;

                _context.HabitLogs.Add(log);

                // Update streak
                var daysSinceLastCompletion = (log.CompletedAt - (habit.LastCompletedAt ?? log.CompletedAt)).Days;
                if (daysSinceLastCompletion <= 1)
                    habit.CurrentStreak++;
                else
                    habit.CurrentStreak = 1;

                habit.LastCompletedAt = log.CompletedAt;
                habit.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return log;
            }
            catch (Exception ex) when (!(ex is NotFoundException || ex is ValidationException))
            {
                _logger.LogError(ex, "Failed to log habit completion for habit {HabitId}", habitId);
                throw new DatabaseException("Failed to log habit completion", ex);
            }
        }

        public async Task<Habit> UpdateHabit(Habit habit)
        {
            try
            {
                _context.Entry(habit).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return habit;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error while updating habit {HabitId}", habit.Id);
                throw new ConcurrencyException("Habit was modified by another user", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while updating habit {HabitId}", habit.Id);
                throw new DatabaseException("Failed to update habit", ex);
            }
        }

        public async Task DeleteHabit(int id)
        {
            try
            {
                var habit = await _context.Habits.FindAsync(id);
                if (habit != null)
                {
                    _context.Habits.Remove(habit);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting habit {HabitId}", id);
                throw new DatabaseException("Failed to delete habit", ex);
            }
        }

        public async Task<bool> ValidateHabitOwnership(int habitId, string userId)
        {
            try
            {
                return await _context.Habits
                    .AnyAsync(h => h.Id == habitId && h.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating habit ownership for habit {HabitId}, user {UserId}",
                    habitId, userId);
                throw;
            }
        }

        public async Task<Dictionary<DateTime, List<HabitCompletion>>> GetHabitCalendar(
            string userId,
            DateTime? from = null,
            DateTime? to = null)
        {
            try
            {
                from ??= DateTime.UtcNow.AddMonths(-1);
                to ??= DateTime.UtcNow;

                var logs = await _context.HabitLogs
                    .Include(l => l.Habit)
                    .Where(l => l.UserId == userId &&
                               l.CompletedAt >= from &&
                               l.CompletedAt <= to)
                    .OrderByDescending(l => l.CompletedAt)
                    .Select(l => new HabitCompletion
                    {
                        HabitId = l.HabitId,
                        HabitTitle = l.Habit.Title,
                        CompletedAt = l.CompletedAt
                    })
                    .ToListAsync();

                return logs.GroupBy(l => l.CompletedAt.Date)
                          .ToDictionary(g => g.Key, g => g.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habit calendar for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<HabitHistory>> GetHabitHistory(
            int habitId,
            string userId,
            DateTime? from = null,
            DateTime? to = null)
        {
            try
            {
                from ??= DateTime.UtcNow.AddMonths(-1);
                to ??= DateTime.UtcNow;

                var logs = await _context.HabitLogs
                    .Where(l => l.HabitId == habitId &&
                               l.UserId == userId &&
                               l.CompletedAt >= from &&
                               l.CompletedAt <= to)
                    .OrderByDescending(l => l.CompletedAt)
                    .ToListAsync();

                var history = new List<HabitHistory>();
                var currentDate = to.Value;
                var streakCount = 0;

                while (currentDate >= from)
                {
                    var log = logs.FirstOrDefault(l => l.CompletedAt.Date == currentDate.Date);

                    if (log != null)
                    {
                        streakCount++;
                    }
                    else
                    {
                        streakCount = 0;
                    }

                    history.Add(new HabitHistory
                    {
                        HabitId = habitId,
                        CompletedAt = currentDate,
                        WasSkipped = streakCount == 0,
                        StreakCount = streakCount
                    });

                    currentDate = currentDate.AddDays(-1);
                }

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get history for habit {HabitId}", habitId);
                throw;
            }
        }

        public async Task<Habit> ArchiveHabit(int habitId, string userId)
        {
            try
            {
                var habit = await GetHabit(habitId, userId);
                if (habit == null)
                    throw new NotFoundException($"Habit {habitId} not found");

                habit.IsArchived = true;
                habit.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return habit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to archive habit {HabitId}", habitId);
                throw;
            }
        }

        public async Task<Habit> UnarchiveHabit(int habitId, string userId)
        {
            try
            {
                var habit = await GetHabit(habitId, userId);
                if (habit == null)
                    throw new NotFoundException($"Habit {habitId} not found");

                await ValidateHabitLimit(userId);

                habit.IsArchived = false;
                habit.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return habit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to unarchive habit {HabitId}", habitId);
                throw;
            }
        }

        public async Task<List<HabitSuggestion>> GetHabitSuggestions(string userId)
        {
            try
            {
                // You might want to move this to a database later
                // The userId parameter could be used to:
                // 1. Filter out suggestions based on user's existing habits
                // 2. Personalize suggestions based on user preferences
                // 3. Track which suggestions have been shown/used

                return new List<HabitSuggestion>
                {
                    new HabitSuggestion
                    {
                        Id = 1,
                        Title = "Daily Water Intake",
                        Description = "Drink 8 glasses of water throughout the day",
                        Category = "Health",
                        DifficultyLevel = "Beginner",
                        EstimatedTimeMinutes = 5,
                        Tags = new List<string> { "health", "hydration", "wellness" },
                        Benefits = "Improved hydration, better skin health, increased energy levels",
                        RecommendedFrequency = "Daily",
                        GettingStartedTips = "Keep a water bottle nearby and set regular reminders"
                    },
                    new HabitSuggestion
                    {
                        Id = 2,
                        Title = "Morning Meditation",
                        Description = "Practice mindfulness meditation in the morning",
                        Category = "Mental Health",
                        DifficultyLevel = "Intermediate",
                        EstimatedTimeMinutes = 10,
                        Tags = new List<string> { "meditation", "mindfulness", "mental health" },
                        Benefits = "Reduced stress, improved focus, better emotional regulation",
                        RecommendedFrequency = "Daily",
                        GettingStartedTips = "Start with guided meditation apps and gradually increase duration"
                    },
                    new HabitSuggestion
                    {
                        Id = 3,
                        Title = "Regular Exercise",
                        Description = "Engage in physical activity",
                        Category = "Fitness",
                        DifficultyLevel = "Intermediate",
                        EstimatedTimeMinutes = 30,
                        Tags = new List<string> { "fitness", "health", "exercise" },
                        Benefits = "Improved strength, better cardiovascular health, weight management",
                        RecommendedFrequency = "3 times per week",
                        GettingStartedTips = "Begin with walking or light exercises and gradually increase intensity"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habit suggestions for user {UserId}", userId);
                throw;
            }
        }

        public async Task<HabitAnalytics> GetHabitAnalytics(string userId, int habitId, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                // Set default date range if not provided
                from ??= DateTime.UtcNow.AddMonths(-1);
                to ??= DateTime.UtcNow;

                //// Get all habit completions for this habit within the date range
                //var completions = await _context.HabitCompletions
                //    .Where(h => h.UserId == userId && 
                //               h.HabitId == habitId &&
                //               h.CompletedAt >= from &&
                //               h.CompletedAt <= to)
                //    .OrderByDescending(h => h.CompletedAt)
                //    .ToListAsync();

                //if (!completions.Any())
                //{
                //    return new HabitAnalytics();
                //}

                var analytics = new HabitAnalytics
                {
                    //TotalCompletions = completions.Count,
                    //LastCompletedAt = completions.FirstOrDefault()?.CompletedAt,
                    DaysTracked = (int)(to.Value - from.Value).TotalDays + 1
                };

                //// Calculate completion rates
                //analytics.CompletionRate = (double)completions.Count / analytics.DaysTracked * 100;

                //// Calculate streaks
                //CalculateStreaks(completions, analytics);

                //// Calculate period-specific completions
                //var startOfWeek = to.Value.Date.AddDays(-(int)to.Value.DayOfWeek);
                //var startOfMonth = new DateTime(to.Value.Year, to.Value.Month, 1);
                //var startOfYear = new DateTime(to.Value.Year, 1, 1);

                //analytics.CompletionsThisWeek = completions.Count(c => c.CompletedAt >= startOfWeek);
                //analytics.CompletionsThisMonth = completions.Count(c => c.CompletedAt >= startOfMonth);
                //analytics.CompletionsThisYear = completions.Count(c => c.CompletedAt >= startOfYear);

                //// Calculate trends
                //CalculateTrends(completions, analytics);

                //// Calculate most productive periods
                //CalculateProductivePeriods(completions, analytics);

                //// Calculate completion breakdowns
                //CalculateCompletionBreakdowns(completions, analytics);

                return analytics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get habit analytics for user {UserId} and habit {HabitId}", userId, habitId);
                throw;
            }
        }

        private void CalculateStreaks(List<HabitCompletion> completions, HabitAnalytics analytics)
        {
            var dates = completions.Select(c => c.CompletedAt.Date).OrderBy(d => d).ToList();
            int currentStreak = 0;
            int longestStreak = 0;
            var today = DateTime.UtcNow.Date;

            // Calculate current streak
            for (var date = today; date >= dates.FirstOrDefault(); date = date.AddDays(-1))
            {
                if (dates.Contains(date))
                {
                    currentStreak++;
                }
                else
                {
                    break;
                }
            }

            // Calculate longest streak
            int tempStreak = 1;
            for (int i = 1; i < dates.Count; i++)
            {
                if (dates[i].AddDays(-1) == dates[i - 1])
                {
                    tempStreak++;
                    longestStreak = Math.Max(longestStreak, tempStreak);
                }
                else
                {
                    tempStreak = 1;
                }
            }

            analytics.CurrentStreak = currentStreak;
            analytics.LongestStreak = Math.Max(longestStreak, 1);
        }

        private void CalculateTrends(List<HabitCompletion> completions, HabitAnalytics analytics)
        {
            var midPoint = completions.Count / 2;
            var recentCompletions = completions.Take(midPoint).Count();
            var olderCompletions = completions.Skip(midPoint).Count();

            analytics.IsImproving = recentCompletions > olderCompletions;
            analytics.TrendDirection = recentCompletions > olderCompletions ? "Up" :
                                     recentCompletions < olderCompletions ? "Down" : "Stable";
        }

        private void CalculateProductivePeriods(List<HabitCompletion> completions, HabitAnalytics analytics)
        {
            var completionsByDay = completions
                .GroupBy(c => c.CompletedAt.DayOfWeek)
                .OrderByDescending(g => g.Count());

            if (completionsByDay.Any())
            {
                analytics.MostProductiveDay = completionsByDay.First().Key;
            }

            var completionsByHour = completions
                .GroupBy(c => new TimeSpan(c.CompletedAt.Hour, 0, 0))
                .OrderByDescending(g => g.Count());

            if (completionsByHour.Any())
            {
                analytics.MostProductiveTimeOfDay = completionsByHour.First().Key;
            }
        }

        private void CalculateCompletionBreakdowns(List<HabitCompletion> completions, HabitAnalytics analytics)
        {
            // Completions by day of week
            var dayNames = Enum.GetNames(typeof(DayOfWeek));
            foreach (var day in dayNames)
            {
                // Implement your logic here to calculate completions by day of week
            }
        }

        public async Task<HabitReport> GenerateHabitReport(string userId, DateTime? from = null, DateTime? to = null, string format = "summary")
        {
            try
            {
                // Set default date range if not provided
                from ??= DateTime.UtcNow.AddMonths(-1);
                to ??= DateTime.UtcNow;

                // Get all user's habits
                var habits = await _context.Habits
                    //.Include(h => h.Stack)
                    .Where(h => h.UserId == userId)
                    //.OrderBy(h => h.Order)
                    .ToListAsync();

                // Get all completions within date range
                //var completions = await _context.HabitCompletions
                //    .Where(c => c.UserId == userId &&
                //               c.CompletedAt >= from &&
                //               c.CompletedAt <= to)
                //    .OrderBy(c => c.CompletedAt)
                //    .ToListAsync();

                var report = new HabitReport
                {
                    ReportStartDate = from.Value,
                    ReportEndDate = to.Value,
                    //TotalCompletions = completions.Count
                };

                //// Generate report based on format
                //switch (format.ToLower())
                //{
                //    case "detailed":
                //        await GenerateDetailedReport(report, habits, completions, from.Value, to.Value);
                //        break;
                //    case "summary":
                //    default:
                //        await GenerateSummaryReport(report, habits, completions, from.Value, to.Value);
                //        break;
                //}

                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate habit report for user {UserId}", userId);
                throw;
            }
        }

        private async Task GenerateSummaryReport(HabitReport report, List<Habit> habits, List<HabitCompletion> completions, DateTime from, DateTime to)
        {
            // Basic statistics
            report.TotalCompletions = completions.Count;
            var totalDays = (to - from).Days + 1;
            report.CompletionRate = (double)completions.Count / (habits.Count * totalDays) * 100;

            // Most active habits
            var completionsByHabit = completions
                .GroupBy(c => c.HabitId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .ToDictionary(g => g.Key, g => g.Count());

            // Calculate streaks
            var currentStreaks = habits.Select(h => CalculateCurrentStreak(h.Id, completions)).Max();
            var longestStreaks = habits.Select(h => CalculateLongestStreak(h.Id, completions)).Max();
            report.CurrentStreak = currentStreaks;
            report.LongestStreak = longestStreaks;

            // Generate basic insights
            GenerateBasicInsights(report, habits, completions);
        }

        private async Task GenerateDetailedReport(HabitReport report, List<Habit> habits, List<HabitCompletion> completions, DateTime from, DateTime to)
        {
            await GenerateSummaryReport(report, habits, completions, from, to);

            // Detailed periodic breakdowns
            CalculatePeriodicBreakdowns(report, completions);

            // Performance metrics
            CalculateDetailedPerformanceMetrics(report, habits, completions, from, to);

            // Trend analysis
            AnalyzeDetailedTrends(report, completions, from, to);

            // Missed days analysis
            AnalyzeMissedDays(report, completions, from, to);

            // Generate detailed insights and recommendations
            GenerateDetailedInsights(report, habits, completions);
        }

        private void CalculatePeriodicBreakdowns(HabitReport report, List<HabitCompletion> completions)
        {
            // By month
            var monthlyCompletions = completions
                .GroupBy(c => new { c.CompletedAt.Year, c.CompletedAt.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month);

            foreach (var month in monthlyCompletions)
            {
                var key = $"{month.Key.Year}-{month.Key.Month:D2}";
                report.CompletionsByMonth[key] = month.Count();
            }

            // By day
        }

        public async Task<List<Habit>> SearchHabits(string userId, HabitSearchParams searchParams)
        {
            try
            {
                // Validate search parameters
                searchParams.ValidatePagination();
                searchParams.ValidateSort();

                // Start with base query
                var query = _context.Habits
                    .Where(h => h.UserId == userId);

                // Apply text search
                if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
                {
                    var searchTerm = searchParams.SearchTerm.ToLower();
                    query = query.Where(h =>
                        h.Title.ToLower().Contains(searchTerm) ||
                        h.Description.ToLower().Contains(searchTerm));
                }

                // Apply filters
                //if (!string.IsNullOrWhiteSpace(searchParams.Category))
                //{
                //    query = query.Where(h => h.Category == searchParams.Category);
                //}

                if (searchParams.IsArchived.HasValue)
                {
                    query = query.Where(h => h.IsArchived == searchParams.IsArchived.Value);
                }

                if (searchParams.IsActive.HasValue)
                {
                    query = query.Where(h => h.IsActive == searchParams.IsActive.Value);
                }

                //// Apply streak filters
                //if (searchParams.MinStreak.HasValue)
                //{
                //    query = query.Where(h => h.CurrentStreak >= searchParams.MinStreak.Value);
                //}

                //if (searchParams.MaxStreak.HasValue)
                //{
                //    query = query.Where(h => h.CurrentStreak <= searchParams.MaxStreak.Value);
                //}

                // Apply date filters
                if (searchParams.CreatedFrom.HasValue)
                {
                    query = query.Where(h => h.CreatedAt >= searchParams.CreatedFrom.Value);
                }

                if (searchParams.CreatedTo.HasValue)
                {
                    query = query.Where(h => h.CreatedAt <= searchParams.CreatedTo.Value);
                }

                if (searchParams.LastCompletedFrom.HasValue)
                {
                    query = query.Where(h => h.LastCompletedAt >= searchParams.LastCompletedFrom.Value);
                }

                if (searchParams.LastCompletedTo.HasValue)
                {
                    query = query.Where(h => h.LastCompletedAt <= searchParams.LastCompletedTo.Value);
                }

                // Apply completion filters
                if (searchParams.MinCompletions.HasValue)
                {
                    query = query.Where(h => h.CompletionCount >= searchParams.MinCompletions.Value);
                }

                if (searchParams.MinCompletionRate.HasValue)
                {
                    // Calculate completion rate based on days since creation
                    query = query.Where(h =>
                        (double)h.CompletionCount /
                        (EF.Functions.DateDiffDay(h.CreatedAt, DateTime.UtcNow) + 1) * 100
                        >= searchParams.MinCompletionRate.Value);
                }

                // Apply tag filters
                if (searchParams.Tags != null && searchParams.Tags.Any())
                {
                    query = query.Where(h => h.Tags.Any(t => searchParams.Tags.Contains(t.Name)));
                }

                // Apply sorting
                query = ApplySorting(query, searchParams);

                // Apply pagination
                var skip = (searchParams.Page - 1) * searchParams.PageSize;
                query = query
                    .Skip(skip)
                    .Take(searchParams.PageSize);

                // Execute query and return results
                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search habits for user {UserId} with params {@SearchParams}",
                    userId, searchParams);
                throw;
            }
        }

        private IQueryable<Habit> ApplySorting(IQueryable<Habit> query, HabitSearchParams searchParams)
        {
            query = searchParams.SortBy?.ToLower() switch
            {
                "title" => searchParams.SortDescending
                    ? query.OrderByDescending(h => h.Title)
                    : query.OrderBy(h => h.Title),

                "currentstreak" => searchParams.SortDescending
                    ? query.OrderByDescending(h => h.CurrentStreak)
                    : query.OrderBy(h => h.CurrentStreak),

                "lastcompletedat" => searchParams.SortDescending
                    ? query.OrderByDescending(h => h.LastCompletedAt)
                    : query.OrderBy(h => h.LastCompletedAt),

                "completioncount" => searchParams.SortDescending
                    ? query.OrderByDescending(h => h.CompletionCount)
                    : query.OrderBy(h => h.CompletionCount),

                // Default sort by CreatedAt
                _ => searchParams.SortDescending
                    ? query.OrderByDescending(h => h.CreatedAt)
                    : query.OrderBy(h => h.CreatedAt)
            };

            return query;
        }

        public async Task<HabitReminder> SetHabitReminder(int habitId, string userId, HabitReminder reminder)
        {
            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.Id == habitId && h.UserId == userId);

            if (habit == null)
            {
                throw new NotFoundException($"Habit {habitId} not found");
            }

            // Get existing reminder or create new one
            var existingReminder = await _context.HabitReminders
                .FirstOrDefaultAsync(r => r.HabitId == habitId && r.UserId == userId);

            if (existingReminder == null)
            {
                existingReminder = new HabitReminder
                {
                    HabitId = habitId,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.HabitReminders.Add(existingReminder);
            }

            // Update reminder properties
            existingReminder.Time = reminder.Time;
            existingReminder.DaysOfWeek = reminder.DaysOfWeek;
            existingReminder.IsEnabled = reminder.IsEnabled;
            existingReminder.NotificationType = reminder.NotificationType;
            existingReminder.NotificationTemplateId = reminder.NotificationTemplateId;
            existingReminder.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingReminder;
        }

        private int CalculateCurrentStreak(int habitId, List<HabitCompletion> completions)
        {
            var habitCompletions = completions
                .Where(c => c.HabitId == habitId && !c.IsSkipped)
                .OrderByDescending(c => c.CompletedAt)
                .ToList();

            if (!habitCompletions.Any())
            {
                return 0;
            }

            var currentStreak = 1;
            var lastDate = habitCompletions[0].CompletedAt.Date;

            for (int i = 1; i < habitCompletions.Count; i++)
            {
                var currentDate = habitCompletions[i].CompletedAt.Date;

                // If dates are consecutive
                if (lastDate.AddDays(-1) == currentDate)
                {
                    currentStreak++;
                    lastDate = currentDate;
                }
                else
                {
                    // Streak is broken
                    break;
                }
            }

            return currentStreak;
        }

        private int CalculateLongestStreak(int habitId, List<HabitCompletion> completions)
        {
            var habitCompletions = completions
                .Where(c => c.HabitId == habitId && !c.IsSkipped)
                .OrderByDescending(c => c.CompletedAt)
                .ToList();

            if (!habitCompletions.Any())
            {
                return 0;
            }

            var longestStreak = 1;
            var currentStreak = 1;
            var lastDate = habitCompletions[0].CompletedAt.Date;

            for (int i = 1; i < habitCompletions.Count; i++)
            {
                var currentDate = habitCompletions[i].CompletedAt.Date;

                // If dates are consecutive
                if (lastDate.AddDays(-1) == currentDate)
                {
                    currentStreak++;
                    longestStreak = Math.Max(longestStreak, currentStreak);
                }
                else
                {
                    // Streak is broken, start new streak
                    currentStreak = 1;
                }

                lastDate = currentDate;
            }

            return longestStreak;
        }

        private void GenerateBasicInsights(HabitReport report, List<Habit> habits, List<HabitCompletion> completions)
        {
            if (!habits.Any() || !completions.Any())
            {
                return;
            }

            // Total completions (excluding skips)
            report.TotalCompletions = completions.Count(c => !c.IsSkipped);

            // Total skips
            report.TotalSkips = completions.Count(c => c.IsSkipped);

            // Most active habit
            var completionsPerHabit = completions
                .Where(c => !c.IsSkipped)
                .GroupBy(c => c.HabitTitle)
                .Select(g => new { Title = g.Key, Count = g.Count() });

            var mostActiveHabit = completionsPerHabit.OrderByDescending(h => h.Count).FirstOrDefault();
            if (mostActiveHabit != null)
            {
                report.MostActiveHabit = mostActiveHabit.Title;
                report.MostActiveHabitCompletions = mostActiveHabit.Count;
            }

            // Most skipped habit
            var skipsPerHabit = completions
                .Where(c => c.IsSkipped)
                .GroupBy(c => c.HabitTitle)
                .Select(g => new { Title = g.Key, Count = g.Count() });

            var mostSkippedHabit = skipsPerHabit.OrderByDescending(h => h.Count).FirstOrDefault();
            if (mostSkippedHabit != null)
            {
                report.MostSkippedHabit = mostSkippedHabit.Title;
                report.MostSkippedHabitCount = mostSkippedHabit.Count;
            }

            // Average completions per day
            var dateRange = (completions.Max(c => c.CompletedAt) - completions.Min(c => c.CompletedAt)).Days + 1;
            report.AverageCompletionsPerDay = dateRange > 0
                ? Math.Round((double)report.TotalCompletions / dateRange, 2)
                : report.TotalCompletions;

            // Completion rate (excluding skips)
            var totalDays = dateRange * habits.Count;
            report.CompletionRate = totalDays > 0
                ? Math.Round((double)report.TotalCompletions / totalDays * 100, 2)
                : 0;

            // Most productive day
            var completionsPerDay = completions
                .Where(c => !c.IsSkipped)
                .GroupBy(c => c.CompletedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() });

            var mostProductiveDay = completionsPerDay.OrderByDescending(d => d.Count).FirstOrDefault();
            if (mostProductiveDay != null)
            {
                report.MostProductiveDate = mostProductiveDay.Date;
                report.MostProductiveDateCompletions = mostProductiveDay.Count;
            }
        }

        private void CalculateDetailedPerformanceMetrics(
            HabitReport report,
            List<Habit> habits,
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            if (!habits.Any() || !completions.Any())
            {
                return;
            }

            // Calculate metrics per habit
            report.HabitMetrics = habits.Select(habit =>
            {
                var habitCompletions = completions
                    .Where(c => c.HabitId == habit.Id)
                    .ToList();

                return new HabitMetric
                {
                    HabitId = habit.Id,
                    Title = habit.Title,
                    TotalCompletions = habitCompletions.Count(c => !c.IsSkipped),
                    TotalSkips = habitCompletions.Count(c => c.IsSkipped),
                    CurrentStreak = CalculateCurrentStreak(habit.Id, completions),
                    LongestStreak = CalculateLongestStreak(habit.Id, completions),
                    CompletionRate = CalculateCompletionRate(habit.Id, habitCompletions, from, to),
                    WeekdayDistribution = CalculateWeekdayDistribution(habitCompletions),
                    TimeOfDayDistribution = CalculateTimeOfDayDistribution(habitCompletions),
                    ConsistencyScore = CalculateConsistencyScore(habitCompletions, from, to)
                };
            }).ToList();

            // Calculate weekly trends
            report.WeeklyTrends = CalculateWeeklyTrends(completions, from, to);

            // Calculate month-over-month growth
            report.MonthlyGrowth = CalculateMonthlyGrowth(completions, from, to);
        }

        private double CalculateCompletionRate(int habitId, List<HabitCompletion> completions, DateTime from, DateTime to)
        {
            var totalDays = (to - from).Days + 1;
            var completedDays = completions
                .Where(c => !c.IsSkipped)
                .Select(c => c.CompletedAt.Date)
                .Distinct()
                .Count();

            return totalDays > 0 ? Math.Round((double)completedDays / totalDays * 100, 2) : 0;
        }

        private Dictionary<DayOfWeek, int> CalculateWeekdayDistribution(List<HabitCompletion> completions)
        {
            return completions
                .Where(c => !c.IsSkipped)
                .GroupBy(c => c.CompletedAt.DayOfWeek)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        private Dictionary<string, int> CalculateTimeOfDayDistribution(List<HabitCompletion> completions)
        {
            return completions
                .Where(c => !c.IsSkipped)
                .GroupBy(c => GetTimeOfDay(c.CompletedAt))
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        private string GetTimeOfDay(DateTime time)
        {
            var hour = time.Hour;
            if (hour >= 5 && hour < 12) return "Morning";
            if (hour >= 12 && hour < 17) return "Afternoon";
            if (hour >= 17 && hour < 21) return "Evening";
            return "Night";
        }

        private double CalculateConsistencyScore(List<HabitCompletion> completions, DateTime from, DateTime to)
        {
            if (!completions.Any()) return 0;

            var totalDays = (to - from).Days + 1;
            var completedDays = completions
                .Where(c => !c.IsSkipped)
                .Select(c => c.CompletedAt.Date)
                .Distinct()
                .Count();

            var streakScore = CalculateLongestStreak(completions.First().HabitId, completions) / (double)totalDays;
            var completionScore = completedDays / (double)totalDays;
            var regularityScore = CalculateRegularityScore(completions, from, to);

            return Math.Round((streakScore + completionScore + regularityScore) / 3 * 100, 2);
        }

        private double CalculateRegularityScore(List<HabitCompletion> completions, DateTime from, DateTime to)
        {
            var completedDates = completions
                .Where(c => !c.IsSkipped)
                .Select(c => c.CompletedAt.Date)
                .OrderBy(d => d)
                .ToList();

            if (!completedDates.Any()) return 0;

            var gaps = new List<int>();
            for (int i = 1; i < completedDates.Count; i++)
            {
                gaps.Add((completedDates[i] - completedDates[i - 1]).Days);
            }

            if (!gaps.Any()) return 1;

            var averageGap = gaps.Average();
            var standardDeviation = CalculateStandardDeviation(gaps, averageGap);

            return 1 / (1 + standardDeviation);
        }

        private double CalculateStandardDeviation(List<int> values, double mean)
        {
            if (!values.Any()) return 0;

            var sumOfSquares = values.Sum(v => Math.Pow(v - mean, 2));
            return Math.Sqrt(sumOfSquares / values.Count);
        }

        private List<WeeklyTrend> CalculateWeeklyTrends(List<HabitCompletion> completions, DateTime from, DateTime to)
        {
            var trends = new List<WeeklyTrend>();
            var currentWeekStart = from.Date;

            while (currentWeekStart <= to)
            {
                var weekEnd = currentWeekStart.AddDays(6);
                var weekCompletions = completions
                    .Where(c => c.CompletedAt.Date >= currentWeekStart &&
                               c.CompletedAt.Date <= weekEnd &&
                               !c.IsSkipped)
                    .ToList();

                trends.Add(new WeeklyTrend
                {
                    WeekStarting = currentWeekStart,
                    CompletionCount = weekCompletions.Count,
                    UniqueHabits = weekCompletions.Select(c => c.HabitId).Distinct().Count()
                });

                currentWeekStart = currentWeekStart.AddDays(7);
            }

            return trends;
        }

        private List<MonthlyGrowth> CalculateMonthlyGrowth(List<HabitCompletion> completions, DateTime from, DateTime to)
        {
            var growth = new List<MonthlyGrowth>();
            var currentMonth = new DateTime(from.Year, from.Month, 1);

            while (currentMonth <= to)
            {
                var monthEnd = currentMonth.AddMonths(1).AddDays(-1);
                var monthCompletions = completions
                    .Where(c => c.CompletedAt.Date >= currentMonth &&
                               c.CompletedAt.Date <= monthEnd &&
                               !c.IsSkipped)
                    .ToList();

                var previousMonth = currentMonth.AddMonths(-1);
                var previousMonthCompletions = completions
                    .Where(c => c.CompletedAt.Date >= previousMonth &&
                               c.CompletedAt.Date < currentMonth &&
                               !c.IsSkipped)
                    .Count();

                var currentMonthCount = monthCompletions.Count;
                var growthRate = previousMonthCompletions > 0
                    ? ((double)currentMonthCount - previousMonthCompletions) / previousMonthCompletions * 100
                    : 0;

                growth.Add(new MonthlyGrowth
                {
                    Month = currentMonth,
                    CompletionCount = currentMonthCount,
                    GrowthRate = Math.Round(growthRate, 2)
                });

                currentMonth = currentMonth.AddMonths(1);
            }

            return growth;
        }

        private void AnalyzeDetailedTrends(
            HabitReport report,
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            if (!completions.Any())
            {
                return;
            }

            report.Trends = new HabitTrends
            {
                // Daily completion patterns
                DailyCompletions = AnalyzeDailyCompletions(completions, from, to),

                // Time patterns
                TimePatterns = AnalyzeTimePatterns(completions),

                // Streak analysis
                StreakPatterns = AnalyzeStreakPatterns(completions, from, to),

                // Completion consistency
                ConsistencyMetrics = AnalyzeConsistency(completions, from, to),

                // Progress indicators
                ProgressIndicators = CalculateProgressIndicators(completions, from, to)
            };
        }

        private List<DailyCompletionPattern> AnalyzeDailyCompletions(
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            var patterns = new List<DailyCompletionPattern>();
            var currentDate = from.Date;

            while (currentDate <= to)
            {
                var dayCompletions = completions
                    .Where(c => c.CompletedAt.Date == currentDate)
                    .ToList();

                patterns.Add(new DailyCompletionPattern
                {
                    Date = currentDate,
                    TotalCompletions = dayCompletions.Count(c => !c.IsSkipped),
                    SkippedCount = dayCompletions.Count(c => c.IsSkipped),
                    UniqueHabits = dayCompletions.Select(c => c.HabitId).Distinct().Count(),
                    DayOfWeek = currentDate.DayOfWeek
                });

                currentDate = currentDate.AddDays(1);
            }

            return patterns;
        }

        private TimePatternAnalysis AnalyzeTimePatterns(List<HabitCompletion> completions)
        {
            var timePatterns = new TimePatternAnalysis
            {
                HourlyDistribution = completions
                    .Where(c => !c.IsSkipped)
                    .GroupBy(c => c.CompletedAt.Hour)
                    .ToDictionary(g => g.Key, g => g.Count()),

                PeakCompletionHour = completions
                    .Where(c => !c.IsSkipped)
                    .GroupBy(c => c.CompletedAt.Hour)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault(),

                WeekdayDistribution = completions
                    .Where(c => !c.IsSkipped)
                    .GroupBy(c => c.CompletedAt.DayOfWeek)
                    .ToDictionary(g => g.Key, g => g.Count()),

                MostProductiveWeekday = completions
                    .Where(c => !c.IsSkipped)
                    .GroupBy(c => c.CompletedAt.DayOfWeek)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
            };

            return timePatterns;
        }

        private StreakAnalysis AnalyzeStreakPatterns(
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            //var streakAnalysis = new StreakAnalysis
            //{
            //    AverageStreakLength = CalculateAverageStreakLength(completions),
            //    StreakDistribution = CalculateStreakDistribution(completions),
            //    LongestStreakStartDate = FindLongestStreakStartDate(completions),
            //    StreakBreakdownByMonth = CalculateMonthlyStreakBreakdown(completions, from, to)
            //};

            //return streakAnalysis;
            throw new NotImplementedException();
        }

        private ConsistencyMetrics AnalyzeConsistency(
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            //var totalDays = (to - from).Days + 1;
            //var completedDays = completions
            //    .Where(c => !c.IsSkipped)
            //    .Select(c => c.CompletedAt.Date)
            //    .Distinct()
            //    .Count();

            //return new ConsistencyMetrics
            //{
            //    OverallConsistency = Math.Round((double)completedDays / totalDays * 100, 2),
            //    ConsecutiveDaysRate = CalculateConsecutiveDaysRate(completions, from, to),
            //    CompletionGapAnalysis = AnalyzeCompletionGaps(completions),
            //    WeeklyConsistencyScores = CalculateWeeklyConsistencyScores(completions, from, to)
            //};
            throw new NotImplementedException();
        }

        private ProgressIndicators CalculateProgressIndicators(
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            //var monthlyData = GetMonthlyCompletionData(completions, from, to);

            //return new ProgressIndicators
            //{
            //    TrendDirection = CalculateTrendDirection(monthlyData),
            //    ImprovementRate = CalculateImprovementRate(monthlyData),
            //    ProjectedCompletion = ProjectNextMonthCompletion(monthlyData),
            //    SuccessRate = CalculateSuccessRate(completions, from, to)
            //};
            throw new NotImplementedException();
        }

        private double CalculateAverageStreakLength(List<HabitCompletion> completions)
        {
            var streaks = GetAllStreaks(completions);
            return streaks.Any() ? Math.Round(streaks.Average(), 2) : 0;
        }

        private Dictionary<int, int> CalculateStreakDistribution(List<HabitCompletion> completions)
        {
            var streaks = GetAllStreaks(completions);
            return streaks
                .GroupBy(length => length)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private List<int> GetAllStreaks(List<HabitCompletion> completions)
        {
            var streaks = new List<int>();
            var currentStreak = 0;
            var lastDate = DateTime.MinValue;

            foreach (var completion in completions.Where(c => !c.IsSkipped)
                                        .OrderBy(c => c.CompletedAt)
                                        .Select(c => c.CompletedAt.Date))
            {
                if (lastDate == DateTime.MinValue || lastDate.AddDays(1) == completion)
                {
                    currentStreak++;
                }
                else
                {
                    if (currentStreak > 0)
                    {
                        streaks.Add(currentStreak);
                    }
                    currentStreak = 1;
                }
                lastDate = completion;
            }

            if (currentStreak > 0)
            {
                streaks.Add(currentStreak);
            }

            return streaks;
        }

        private double CalculateConsecutiveDaysRate(
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            var totalDays = (to - from).Days + 1;
            var consecutiveDays = GetAllStreaks(completions).Sum();

            return totalDays > 0 ? Math.Round((double)consecutiveDays / totalDays * 100, 2) : 0;
        }

        private Dictionary<string, double> AnalyzeCompletionGaps(List<HabitCompletion> completions)
        {
            var dates = completions
                .Where(c => !c.IsSkipped)
                .OrderBy(c => c.CompletedAt)
                .Select(c => c.CompletedAt.Date)
                .ToList();

            var gaps = new List<int>();
            for (int i = 1; i < dates.Count; i++)
            {
                gaps.Add((dates[i] - dates[i - 1]).Days - 1);
            }

            return new Dictionary<string, double>
            {
                { "AverageGap", gaps.Any() ? Math.Round(gaps.Average(), 2) : 0 },
                { "MaxGap", gaps.Any() ? gaps.Max() : 0 },
                { "MedianGap", gaps.Any() ? CalculateMedian(gaps) : 0 }
            };
        }

        private double CalculateMedian(List<int> values)
        {
            var sortedValues = values.OrderBy(v => v).ToList();
            var mid = sortedValues.Count / 2;

            return sortedValues.Count % 2 == 0
                ? (sortedValues[mid - 1] + sortedValues[mid]) / 2.0
                : sortedValues[mid];
        }

        private string CalculateTrendDirection(Dictionary<DateTime, int> monthlyData)
        {
            if (!monthlyData.Any() || monthlyData.Count < 2)
                return "Insufficient data";

            var trend = monthlyData.OrderBy(kvp => kvp.Key)
                .Select(kvp => kvp.Value)
                .ToList();

            var lastThreeMonths = trend.TakeLast(3).ToList();

            if (lastThreeMonths.Count < 2)
                return "Insufficient data";

            if (lastThreeMonths[^1] > lastThreeMonths[^2])
                return "Upward";
            if (lastThreeMonths[^1] < lastThreeMonths[^2])
                return "Downward";

            return "Stable";
        }

        private Dictionary<DateTime, int> GetMonthlyCompletionData(
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            return completions
                .Where(c => !c.IsSkipped)
                .GroupBy(c => new DateTime(c.CompletedAt.Year, c.CompletedAt.Month, 1))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private void AnalyzeMissedDays(
            HabitReport report,
            List<HabitCompletion> completions,
            DateTime from,
            DateTime to)
        {
            if (!completions.Any())
            {
                return;
            }

            var completedDates = completions
                .Where(c => !c.IsSkipped)
                .Select(c => c.CompletedAt.Date)
                .Distinct()
                .ToHashSet();

            var missedDays = new List<DateTime>();
            var currentDate = from.Date;

            // Find all missed days
            while (currentDate <= to)
            {
                if (!completedDates.Contains(currentDate))
                {
                    missedDays.Add(currentDate);
                }
                currentDate = currentDate.AddDays(1);
            }

            report.MissedDaysAnalysis = new MissedDaysAnalysis
            {
                TotalMissedDays = missedDays.Count,

                // Missed days percentage
                MissedDaysRate = Math.Round(
                    (double)missedDays.Count / ((to - from).Days + 1) * 100,
                    2),

                // Most common missed day of week
                MostMissedWeekday = missedDays
                    .GroupBy(d => d.DayOfWeek)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault(),

                // Longest streak of missed days
                LongestMissedStreak = CalculateLongestMissedStreak(missedDays),

                // Average gap between missed days
                AverageGapBetweenMissedDays = CalculateAverageGapBetweenMissedDays(missedDays),

                // Monthly breakdown of missed days
                MonthlyMissedDays = CalculateMonthlyMissedDays(missedDays, from, to),

                // Recent trend (last 30 days)
                RecentMissedDaysTrend = CalculateRecentMissedDaysTrend(missedDays, to)
            };
        }

        private int CalculateLongestMissedStreak(List<DateTime> missedDays)
        {
            if (!missedDays.Any())
                return 0;

            var orderedDays = missedDays.OrderBy(d => d).ToList();
            var currentStreak = 1;
            var longestStreak = 1;

            for (int i = 1; i < orderedDays.Count; i++)
            {
                if (orderedDays[i] == orderedDays[i - 1].AddDays(1))
                {
                    currentStreak++;
                    longestStreak = Math.Max(longestStreak, currentStreak);
                }
                else
                {
                    currentStreak = 1;
                }
            }

            return longestStreak;
        }

        private double CalculateAverageGapBetweenMissedDays(List<DateTime> missedDays)
        {
            if (missedDays.Count <= 1)
                return 0;

            var orderedDays = missedDays.OrderBy(d => d).ToList();
            var gaps = new List<int>();

            for (int i = 1; i < orderedDays.Count; i++)
            {
                gaps.Add((orderedDays[i] - orderedDays[i - 1]).Days);
            }

            return Math.Round(gaps.Average(), 2);
        }

        private Dictionary<string, int> CalculateMonthlyMissedDays(
            List<DateTime> missedDays,
            DateTime from,
            DateTime to)
        {
            return missedDays
                .GroupBy(d => $"{d.Year}-{d.Month:D2}")
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        private MissedDaysTrend CalculateRecentMissedDaysTrend(List<DateTime> missedDays, DateTime to)
        {
            var thirtyDaysAgo = to.AddDays(-30);
            var recentMissedDays = missedDays.Count(d => d >= thirtyDaysAgo && d <= to);
            var previousThirtyDays = missedDays.Count(d =>
                d >= thirtyDaysAgo.AddDays(-30) &&
                d < thirtyDaysAgo);

            return new MissedDaysTrend
            {
                LastThirtyDaysMissed = recentMissedDays,
                PreviousThirtyDaysMissed = previousThirtyDays,
                TrendDirection = recentMissedDays.CompareTo(previousThirtyDays) switch
                {
                    < 0 => "Improving",
                    > 0 => "Declining",
                    _ => "Stable"
                }
            };
        }

        private void GenerateDetailedInsights(
            HabitReport report,
            List<Habit> habits,
            List<HabitCompletion> completions)
        {
            if (!habits.Any() || !completions.Any())
            {
                return;
            }

            report.DetailedInsights = new DetailedInsights
            {
                // Habit-specific insights
                HabitInsights = GenerateHabitSpecificInsights(habits, completions),

                // Time-based patterns
                TimePatterns = GenerateTimePatterns(completions),

                // Success patterns
                SuccessPatterns = GenerateSuccessPatterns(completions),

                // Improvement suggestions
                Suggestions = GenerateImprovementSuggestions(habits, completions)
            };
        }

        private List<HabitInsight> GenerateHabitSpecificInsights(
            List<Habit> habits,
            List<HabitCompletion> completions)
        {
            return habits.Select(habit =>
            {
                var habitCompletions = completions
                    .Where(c => c.HabitId == habit.Id)
                    .ToList();

                return new HabitInsight
                {
                    HabitId = habit.Id,
                    Title = habit.Title,
                    CompletionCount = habitCompletions.Count(c => !c.IsSkipped),
                    SkipCount = habitCompletions.Count(c => c.IsSkipped),
                    SuccessRate = CalculateSuccessRate(habitCompletions),
                    BestPerformingDays = GetBestPerformingDays(habitCompletions),
                    AverageCompletionsPerWeek = CalculateAverageCompletionsPerWeek(habitCompletions)
                };
            }).ToList();
        }

        private TimePatternInsights GenerateTimePatterns(List<HabitCompletion> completions)
        {
            //var nonSkipped = completions.Where(c => !c.IsSkipped).ToList();

            //return new TimePatternInsights
            //{
            //    MostProductiveTimeOfDay = GetMostProductiveTimeOfDay(nonSkipped),
            //    DayOfWeekDistribution = GetDayOfWeekDistribution(nonSkipped),
            //    WeekendVsWeekday = CalculateWeekendVsWeekday(nonSkipped),
            //    ConsistentTimeSlots = IdentifyConsistentTimeSlots(nonSkipped)
            //};
            throw new NotImplementedException();

        }

        private SuccessPatternInsights GenerateSuccessPatterns(List<HabitCompletion> completions)
        {
            //var nonSkipped = completions.Where(c => !c.IsSkipped).ToList();

            //return new SuccessPatternInsights
            //{
            //    ConsecutiveDayPatterns = AnalyzeConsecutiveDayPatterns(nonSkipped),
            //    SuccessfulSequences = IdentifySuccessfulSequences(nonSkipped),
            //    FailurePatterns = IdentifyFailurePatterns(completions),
            //    RecoveryPatterns = AnalyzeRecoveryPatterns(completions)
            //};
            throw new NotImplementedException();

        }

        private List<string> GenerateImprovementSuggestions(
            List<Habit> habits,
            List<HabitCompletion> completions)
        {
            var suggestions = new List<string>();

            // Analyze completion patterns
            foreach (var habit in habits)
            {
                var habitCompletions = completions.Where(c => c.HabitId == habit.Id).ToList();

                // Low completion rate suggestion
                if (CalculateSuccessRate(habitCompletions) < 50)
                {
                    suggestions.Add($"Consider breaking down '{habit.Title}' into smaller, more manageable steps.");
                }

                // Inconsistent timing suggestion
                if (!HasConsistentTiming(habitCompletions))
                {
                    suggestions.Add($"Try completing '{habit.Title}' at the same time each day for better consistency.");
                }

                // Weekend drop-off suggestion
                if (HasWeekendDropoff(habitCompletions))
                {
                    suggestions.Add($"Your completion rate for '{habit.Title}' drops on weekends. Consider adjusting your weekend routine.");
                }
            }

            // Overall suggestions
            if (HasLongGapsBetweenCompletions(completions))
            {
                suggestions.Add("Long gaps between completions detected. Try to maintain momentum with smaller steps during challenging periods.");
            }

            if (HasHighSkipRate(completions))
            {
                suggestions.Add("Consider reviewing habit difficulty levels as skip rates are high.");
            }

            return suggestions;
        }

        private double CalculateSuccessRate(List<HabitCompletion> completions)
        {
            if (!completions.Any())
                return 0;

            return Math.Round(
                (double)completions.Count(c => !c.IsSkipped) / completions.Count * 100,
                2);
        }

        private List<DayOfWeek> GetBestPerformingDays(List<HabitCompletion> completions)
        {
            return completions
                .Where(c => !c.IsSkipped)
                .GroupBy(c => c.CompletedAt.DayOfWeek)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();
        }

        private double CalculateAverageCompletionsPerWeek(List<HabitCompletion> completions)
        {
            if (!completions.Any())
                return 0;

            var weeks = (completions.Max(c => c.CompletedAt) - completions.Min(c => c.CompletedAt)).Days / 7.0;
            if (weeks == 0)
                return completions.Count;

            return Math.Round(completions.Count(c => !c.IsSkipped) / weeks, 2);
        }

        private bool HasConsistentTiming(List<HabitCompletion> completions)
        {
            var timeSlots = completions
                .Where(c => !c.IsSkipped)
                .GroupBy(c => c.CompletedAt.Hour)
                .OrderByDescending(g => g.Count());

            var totalCompletions = completions.Count(c => !c.IsSkipped);
            return timeSlots.Any(g => (double)g.Count() / totalCompletions > 0.6);
        }

        private bool HasWeekendDropoff(List<HabitCompletion> completions)
        {
            var weekdayAvg = completions
                .Where(c => !c.IsSkipped && !IsWeekend(c.CompletedAt))
                .GroupBy(c => c.CompletedAt.Date)
                .Average(g => g.Count());

            var weekendAvg = completions
                .Where(c => !c.IsSkipped && IsWeekend(c.CompletedAt))
                .GroupBy(c => c.CompletedAt.Date)
                .Average(g => g.Count());

            return weekendAvg < weekdayAvg * 0.5;
        }

        private bool HasLongGapsBetweenCompletions(List<HabitCompletion> completions)
        {
            var dates = completions
                .Where(c => !c.IsSkipped)
                .OrderBy(c => c.CompletedAt)
                .Select(c => c.CompletedAt.Date)
                .ToList();

            for (int i = 1; i < dates.Count; i++)
            {
                if ((dates[i] - dates[i - 1]).Days > 7)
                    return true;
            }

            return false;
        }

        private bool HasHighSkipRate(List<HabitCompletion> completions)
        {
            if (!completions.Any())
                return false;

            return (double)completions.Count(c => c.IsSkipped) / completions.Count > 0.3;
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}