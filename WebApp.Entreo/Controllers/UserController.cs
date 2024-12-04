using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Entreo.Infrastructure.Auth;
using WebApp.Entreo.Services.Interfaces;
using WebApp.Entreo.Shared.Models.DTOs;

namespace WebApp.Entreo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserAccessor _userAccessor;

        public UserController(
            IUserService userService,
            IUserAccessor userAccessor)
        {
            _userService = userService;
            _userAccessor = userAccessor;
        }

        [HttpGet("current")]
        public ActionResult<UserDto> GetCurrentUser()
        {
            var userId = _userAccessor.GetCurrentUserId();
            var username = _userAccessor.GetCurrentUsername();

            return new UserDto
            {
                Id = userId,
                UserName = username,
                IsAuthenticated = _userAccessor.IsAuthenticated()
            };
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName
            }).ToList();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUser(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserDto>> UpdateUser(string userId, UserUpdateDto updateDto)
        {
            if (userId != _userAccessor.GetCurrentUserId() && !User.IsInRole("Admin"))
                return Forbid();

            var updatedUser = await _userService.UpdateUserAsync(userId, updateDto);
            if (updatedUser == null)
                return NotFound();

            return new UserDto
            {
                Id = updatedUser.Id,
                UserName = updatedUser.UserName,
                Email = updatedUser.Email,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName
            };
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(false);

            var result = await _userService.DeleteUserAsync(userId);
            if (!result)
                return NotFound(false);

            return true;
        }

        [HttpDelete("current")]
        public async Task<ActionResult<bool>> DeleteCurrentUser()
        {
            var currentUserId = _userAccessor.GetCurrentUserId();
            var result = await _userService.DeleteUserAsync(currentUserId);

            if (!result)
                return BadRequest(false);

            return true;
        }

        [HttpPost("{userId}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeactivateUser(string userId)
        {
            var result = await _userService.DeactivateUserAsync(userId);
            if (!result)
                return NotFound(false);

            return true;
        }

        [HttpPost("{userId}/reactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> ReactivateUser(string userId)
        {
            var result = await _userService.ReactivateUserAsync(userId);
            if (!result)
                return NotFound(false);

            return true;
        }
    }
}