using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Entreo.Exceptions;
using WebApp.Entreo.Infrastructure.Auth;
using WebApp.Entreo.Models;
using WebApp.Entreo.Services;

namespace WebApp.Entreo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HabitLogController : ControllerBase
    {
        private readonly HabitLogService _logService;
        private readonly ILogger<HabitLogController> _logger;
        private readonly IUserAccessor _userAccessor;

        public HabitLogController(
            HabitLogService logService,
            ILogger<HabitLogController> logger,
            IUserAccessor userAccessor)
        {
            _logService = logService;
            _logger = logger;
            _userAccessor = userAccessor;
        }

        [HttpGet]
        public async Task<List<HabitLog>> GetLogs([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _logService.GetUserLogs(userId, from, to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get logs for user {UserId}", _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpGet("habit/{habitId}")]
        public async Task<List<HabitLog>> GetHabitLogs(int habitId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                return await _logService.GetHabitLogs(habitId, userId, from, to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get logs for habit {HabitId}, user {UserId}",
                    habitId, _userAccessor.GetCurrentUserId());
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteLog(int id)
        {
            try
            {
                var userId = _userAccessor.GetCurrentUserId();
                var log = await _logService.GetLog(id);

                if (log == null || log.UserId != userId)
                {
                    _logger.LogWarning("Delete attempted on non-existent or unauthorized log {LogId}", id);
                    throw new NotFoundException($"Log {id} not found");
                }

                await _logService.DeleteLog(id, userId);
            }
            catch (Exception ex) when (!(ex is NotFoundException))
            {
                _logger.LogError(ex, "Failed to delete log {LogId} for user {UserId}",
                    id, _userAccessor.GetCurrentUserId());
                throw;
            }
        }
    }
}