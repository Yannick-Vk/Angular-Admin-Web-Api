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
    public async Task AddRole(RoleDto role) {
        await service.AddRole(new Role(role.roleName) );
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteRole(RoleDto role) {
        var result = await service.DeleteRole(role.roleName);
        if (result.Succeeded) {
            return Ok();
        }

        result.Errors.ToList().ForEach(error => logger.LogError("[{code}]: {msg}",  error.Code, error.Description));
        return BadRequest(result.Errors.FirstOrDefault());
    }

    [HttpGet("add-to-user")]
    public async Task AddUserToRole(AddRoleToUserDto dto) {
        await service.AddRoleToUser(dto.RoleName, dto.Username);
    }
}