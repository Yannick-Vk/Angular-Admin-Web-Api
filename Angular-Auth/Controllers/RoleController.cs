using Angular_Auth.Dto;
using Angular_Auth.Models;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/roles")]
public class RoleController(ILogger<RoleController> logger, IRoleService service) : Controller {
    [HttpGet]
    public async Task<List<IdentityRole>> GetRoles() {
        return (await service.GetAllRoles()).ToList();
    }

    [HttpPost]
    public async Task CreateNewRole(RoleDto role) {
        await service.CreateNewRole(new Role(role.roleName));
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteRole(RoleDto role) {
        var result = await service.DeleteRole(role.roleName);
        if (result.Succeeded) {
            return Ok();
        }

        result.Errors.ToList().ForEach(error => logger.LogError("[{code}]: {msg}", error.Code, error.Description));
        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpPost("add-to-user")]
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
    public async Task<List<UserDto>> GetUserWithRole(string roleName) {
        var users = await service.GetUsersWithRole(roleName);
        return users.ToList();
    }

    [HttpGet("{roleName}/{username}")]
    public async Task<ActionResult<bool>> UserHasRole(string roleName, string username) {
        try {
            return await service.UserHasRole(roleName, username);
        }
        catch (ArgumentException e) {
            return BadRequest(e.Message);
        }
    }
}