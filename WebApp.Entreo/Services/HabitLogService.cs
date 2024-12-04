using Microsoft.EntityFrameworkCore;
using WebApp.Entreo.Data;
using WebApp.Entreo.Exceptions;
using WebApp.Entreo.Models;

namespace WebApp.Entreo.Services
{
    public class HabitLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HabitLogService> _logger;

        public HabitLogService(ApplicationDbContext context, ILogger<HabitLogService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<HabitLog>> GetUserLogs(string userId, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var query = _context.HabitLogs
                    .AsNoTracking()
                    .Where(l => l.UserId == userId);

                if (from.HasValue)
                {
                    query = query.Where(l => l.CreatedAt >= from.Value);
                }

                if (to.HasValue)
                {
                    query = query.Where(l => l.CreatedAt <= to.Value);
                }

                return await query
                    .OrderByDescending(l => l.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get logs for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<HabitLog>> GetHabitLogs(int habitId, string userId, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                // Verify habit ownership
                var habitExists = await _context.Habits
                    .AnyAsync(h => h.Id == habitId && h.UserId == userId);

                if (!habitExists)
                    throw new NotFoundException($"Habit {habitId} not found");

                var query = _context.HabitLogs
                    .Where(l => l.HabitId == habitId && l.UserId == userId);

                if (from.HasValue)
                    query = query.Where(l => l.CompletedAt >= from.Value);

                if (to.HasValue)
                    query = query.Where(l => l.CompletedAt <= to.Value);

                return await query
                    .OrderByDescending(l => l.CompletedAt)
                    .ToListAsync();
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Failed to get logs for habit {HabitId}, user {UserId}, from {From} to {To}",
                    habitId, userId, from, to);
                throw;
            }
        }

        public async Task DeleteLog(int id, string userId)
        {
            try
            {
                var log = await _context.HabitLogs
                    .Include(l => l.Habit)
                    .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

                if (log == null)
                {
                    throw new NotFoundException($"Log {id} not found");
                }

                _context.HabitLogs.Remove(log);

                // Update habit statistics
                if (log.Habit != null)
                {
                    log.Habit.CompletionCount--;
                    if (log.Habit.LastCompletedAt == log.CreatedAt)
                    {
                        // Find the new last completion date
                        var lastCompletion = await _context.HabitLogs
                            .Where(l => l.HabitId == log.HabitId && l.Id != id)
                            .OrderByDescending(l => l.CreatedAt)
                            .FirstOrDefaultAsync();

                        log.Habit.LastCompletedAt = lastCompletion?.CreatedAt;
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete log {LogId} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<HabitLog> GetLog(int id)
        {
            return await _context.HabitLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}