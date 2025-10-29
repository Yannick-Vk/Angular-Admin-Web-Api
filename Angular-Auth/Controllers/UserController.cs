using System.Security.Claims;
using Angular_Auth.Dto.Users;
using Angular_Auth.Exceptions;
using Angular_Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("/api/v1/users")]
[Authorize]
public class UserController(IUserService service, IRoleService roleService) : ControllerBase {
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<IActionResult> GetUsers() {
        return Ok(await service.GetUsers());
    }

    [HttpGet("find/{userName}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<IActionResult> GetUserWithUsername(string userName) {
        try {
            var fullUser = await service.GetUserByUsername(userName);
            return Ok(new UserDto(fullUser));
        }
        catch (UserNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<IActionResult> GetUserById(string userId) {
        try {
            var fullUser = await service.GetUserById(userId);
            return Ok(new UserDto(fullUser));
        }
        catch (UserNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("me")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<IActionResult> GetUserData() {
        try {
            var userId = User.FindFirstValue("Id");
            if (userId == null) {
                return Unauthorized("Could not get userId from JWT Token");
            }

            var fullUser = await service.GetUserById(userId);
            return Ok(new UserDto(fullUser));
        }
        catch (UserNotFoundException ex) {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{userName}/Roles")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<IActionResult> GetUserRoles(string userName) {
        try {
            return Ok(await roleService.GetRolesFromUser(userName));
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{userId}/profile-picture")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<IActionResult> GetUserProfilePicture(string userId) {
        try {
            var image = await service.GetUserProfilePicture(userId);
            if (image.Length == 0) return NotFound();

            return new FileContentResult(image, "image/jpeg");
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}