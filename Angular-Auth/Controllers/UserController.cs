using Angular_Auth.Models;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("/api/v1/users")]
public class UserController(IUserService service) : ControllerBase {
    [HttpGet]
    public async Task<List<User>> GetUsers() {
        return await service.GetUsers();
    }
}