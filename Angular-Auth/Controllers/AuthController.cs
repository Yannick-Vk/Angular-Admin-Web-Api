using Angular_Auth.Dto;
using Angular_Auth.Exceptions;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class AuthController(ILogger<AuthController> logger, IAuthenticationService service) : ControllerBase {
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) {
        try {
            return Ok(await service.Login(request));
        }
        catch (Exception e) when (e is CredentialsRequiredException or WrongCredentialsException) {
            return BadRequest(e.Message);
        }
        catch (Exception e) {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request) {
        try {
            return Ok(await service.Register(request));
        }
        catch (Exception e) {
            return BadRequest(e.Message);
        }
    }
}