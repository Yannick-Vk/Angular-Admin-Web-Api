using Angular_Auth.Dto;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("/api/v1/users")]
public class UserController(IUserService service, IRoleService roleService) : ControllerBase {
    [HttpGet]
    public async Task<List<UserDto>> GetUsers() {
        return await service.GetUsers();
    }

    [HttpGet("{userName}")]
    public async Task<UserDto?> GetUser(string userName) {
        return await service.GetUserDto(userName);
    }

    [HttpGet("{userName}/Roles")]
    public async Task<IActionResult> GetUserRoles(string userName) {
        try {
            return Ok(await roleService.GetRolesFromUser(userName));
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}