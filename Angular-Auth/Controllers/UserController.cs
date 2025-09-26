using Angular_Auth.Dto;
using Angular_Auth.Exceptions;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("/api/v1/users")]
[Authorize(Roles = "Admin")]
public class UserController(IUserService service, IRoleService roleService) : ControllerBase {
    [HttpGet]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    public async Task<IActionResult> GetUsers() {
        return Ok(await service.GetUsers());
    }

    [HttpGet("find/{userName}")]
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

    [HttpGet("{userName}/Roles")]
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
}