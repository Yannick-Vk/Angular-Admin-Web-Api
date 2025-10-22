using System.Security.Claims;
using Angular_Auth.Dto;
using Angular_Auth.Dto.Auth;
using Angular_Auth.Exceptions;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.WebIntegration;
using IAuthenticationService = Angular_Auth.Services.Interfaces.IAuthenticationService;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    IAuthenticationService service,
    IConfiguration configuration) : ControllerBase {
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) {
        try {
            var resp = await service.Login(request);
            service.SetTokenCookie(HttpContext, resp.Token, resp.RefreshToken);
            return Ok(LoginResponse.FromResponseWithToken(resp));
        }
        catch (Exception e) when (e is CredentialsRequiredException or WrongCredentialsException
                                      or EmailNotVerifiedException) {
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
            var resp = await service.Register(request);
            service.SetTokenCookie(HttpContext, resp.Token, resp.RefreshToken);
            return Ok(resp.RemoveToken());
        }
        catch (Exception e) {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout() {
        service.RemoveTokenCookie(HttpContext);
        return Ok(new { message = "Logged out successfully" });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<IActionResult> RefreshToken() {
        try {
            var refreshToken = HttpContext.Request.Cookies["refreshToken"];
            var resp = await service.RefreshToken(refreshToken);
            service.SetTokenCookie(HttpContext, resp.Token, resp.RefreshToken);
            return Ok(LoginResponse.FromResponseWithToken(resp));
        }
        catch (Exception e) when (e is CredentialsRequiredException or WrongCredentialsException) {
            return BadRequest(e.Message);
        }
        catch (Exception e) {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string userId, [FromQuery] string token) {
        var result = await service.VerifyEmail(userId, token);

        return Redirect(result
            ? configuration["front-end:email-confirmed"]!
            : configuration["front-end:email-verification-failed"]!
        );
    }

    [AllowAnonymous]
    [HttpGet("challenge")]
    public IResult ChallengeSomething() {
        logger.LogInformation("Challenge Something");
        return Results.Challenge(
            properties: new AuthenticationProperties { RedirectUri = "/api/v1/Auth/whoami" },
            authenticationSchemes: [OpenIddictClientWebIntegrationConstants.Providers.GitHub]);
    }

    [AllowAnonymous]
    [HttpGet("callback/login/github")]
    public async Task<IActionResult> GithubCallbackLogin() {
        logger.LogInformation("Github callback login");
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientWebIntegrationConstants.Providers.GitHub);

        var email = result.Principal!.GetClaim(ClaimTypes.Email);
        var name = result.Principal!.GetClaim(ClaimTypes.Name);

        if (email is null || name is null) {
            return BadRequest("Could not retrieve user information from GitHub.");
        }

        var (user, token) = await service.LoginWithProvider(email, name, "GitHub");
        logger.LogInformation("Logged in with user {user}", user.UserName);
        
        service.SetTokenCookie(HttpContext, token.Token, token.RefreshToken);

        var redirectUri = result.Properties?.RedirectUri ?? configuration["front-end:base-url"] ?? "/";
        
        logger.LogInformation("Redirecting to {redirectUri}", redirectUri);

        return Redirect(redirectUri);
    }

    [AllowAnonymous]
    [HttpGet("whoami")]
    public async Task<IActionResult> WhoAmI() {
        var result = await HttpContext.AuthenticateAsync();
        if (result is not { Succeeded: true }) {
            return Ok("You're not logged in.");
        }

        return Ok($"You are {result.Principal.FindFirst(ClaimTypes.Name)!.Value}.");
    }
}