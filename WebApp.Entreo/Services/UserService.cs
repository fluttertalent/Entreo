using Microsoft.EntityFrameworkCore;
using WebApp.Entreo.Data;
using WebApp.Entreo.Infrastructure.Auth;
using WebApp.Entreo.Services.Interfaces;
using WebApp.Entreo.Shared.Models;
using WebApp.Entreo.Shared.Models.DTOs;

namespace WebApp.Entreo.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public UserService(
            ApplicationDbContext context,
            IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                // Verify the user exists
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return false;

                // Delete related records in the correct order to avoid foreign key conflicts
                await DeleteUserRelatedRecordsAsync(userId);

                // Finally delete the user
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }

        private async Task DeleteUserRelatedRecordsAsync(string userId)
        {
            // Delete HabitLogs
            var habitLogs = await _context.HabitLogs
                .Where(h => h.UserId == userId)
                .ToListAsync();
            _context.HabitLogs.RemoveRange(habitLogs);

            // Delete HabitReminders
            var habitReminders = await _context.HabitReminders
                .Where(h => h.UserId == userId)
                .ToListAsync();
            _context.HabitReminders.RemoveRange(habitReminders);

            // Delete Habits
            var habits = await _context.Habits
                .Where(h => h.UserId == userId)
                .ToListAsync();
            _context.Habits.RemoveRange(habits);

            // Delete UserNotificationPreferences
            var notificationPreferences = await _context.UserNotificationPreferences
                .Where(n => n.UserId == userId)
                .ToListAsync();
            _context.UserNotificationPreferences.RemoveRange(notificationPreferences);

            // Delete PushNotificationTokens
            var pushTokens = await _context.PushNotificationTokens
                .Where(p => p.UserId == userId)
                .ToListAsync();
            _context.PushNotificationTokens.RemoveRange(pushTokens);

            // Save changes after deleting related records
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _context.Users
                .Include(u => u.Habits)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Habits)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<bool> UpdateUserAsync(string userId, UserUpdateDto updateDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Update user properties
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.Email = updateDto.Email;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ReactivateUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        Task<User> IUserService.UpdateUserAsync(string userId, UserUpdateDto updateDto)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateUserRolesAsync(string userId, IEnumerable<string> roles)
        {
            throw new NotImplementedException();
        }
    }
}