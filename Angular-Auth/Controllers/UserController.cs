using Angular_Auth.Dto;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/users")]
public class UserController(IUserService service, IRoleService roleService) : ControllerBase {
    [HttpGet]
    public async Task<List<UserDto>> GetUsers() {
        return await service.GetUsers();
    }

    [HttpGet("{userName}")]
    public async Task<UserDto?> GetUser(string userName) {
        return await service.GetUser(userName);
    }
    
    [HttpGet("{userName}/Roles")]
    public async Task<IEnumerable<string>> GetUserRoles(string userName) {
        return await roleService.GetRolesFromUser(userName);
    }
}