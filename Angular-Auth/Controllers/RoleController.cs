using Angular_Auth.Dto;
using Angular_Auth.Dto.Users;
using Angular_Auth.Models;
using Angular_Auth.Services;
using Angular_Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/roles")]
public class RoleController(ILogger<RoleController> logger, IRoleService service, IAuthenticationService authService)
    : Controller {
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<List<IdentityRole>> GetRoles() {
        return (await service.GetAllRoles()).ToList();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task CreateNewRole(RoleDto role) {
        await service.CreateNewRole(new Role(role.roleName));
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<ActionResult> DeleteRole(RoleDto role) {
        var result = await service.DeleteRole(role.roleName);
        if (result.Succeeded) return Ok();

        result.Errors.ToList().ForEach(error => logger.LogError("[{code}]: {msg}", error.Code, error.Description));
        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpPost("add-to-user")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<ActionResult> AddUserToRole(UserAndRoleDto dto) {
        try {
            await service.AddRoleToUser(dto.RoleName, dto.Username);
            return Ok();
        }
        catch (ArgumentException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("remove-from-user")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<ActionResult> RemoveRoleFromUser(UserAndRoleDto dto) {
        try {
            await service.RemoveRoleFromUser(dto.RoleName, dto.Username);
            return Ok();
        }
        catch (ArgumentException e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{roleName}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<List<UserDto>> GetUserWithRole(string roleName) {
        var users = await service.GetUsersWithRole(roleName);
        return users.ToList();
    }

    [HttpGet("me/{roleName}")]
    [Authorize]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status404NotFound)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<ActionResult<bool>> UserHasRole(string roleName) {
        try {
            var user = authService.GetUserWithRolesFromClaimsPrincipal(HttpContext.User);
            if (user is null) return Unauthorized();
            return await service.UserHasRole(roleName, user);
        }
        catch (ArgumentException e) {
            return NotFound(e.Message);
        }
    }
}