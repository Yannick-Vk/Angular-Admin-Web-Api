using Angular_Auth.Models;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/roles")]
public class RoleController(IRoleService service) : Controller {
    [HttpGet]
    public async Task<List<IdentityRole>> GetRoles() {
        return (await service.GetAllRoles()).ToList();
    }

    [HttpPost]
    public async Task AddRole(string roleName) {
        await service.AddRole(roleName);
    }
}