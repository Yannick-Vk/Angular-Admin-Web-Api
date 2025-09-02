using Angular_Auth.Dto;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("/api/v1/auth")]
public class AuthController(ILogger<AuthController> logger, IAuthenticationService service) : ControllerBase {
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(LoginResponse))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) {
        var response = await service.Login(request);
        if (!response.Success) {
            logger.LogWarning("Login failed {err}", response.Message);
            return Unauthorized(response);
        }
        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(LoginResponse))]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request) {
        var response = await service.Register(request);
        if (!response.Success) {
            logger.LogWarning("Registration failed {err}", response.Message);
            return Unauthorized(response);
        }
        return Ok(response);
    }
}