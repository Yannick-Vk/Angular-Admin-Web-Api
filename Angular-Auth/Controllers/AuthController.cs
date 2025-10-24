using System.Security.Claims;
using Angular_Auth.Dto.Auth;
using Angular_Auth.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Client.WebIntegration;
using IAuthenticationService = Angular_Auth.Services.Interfaces.IAuthenticationService;
using Angular_Auth.Services.Interfaces;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class AuthController(
    ILogger<AuthController> logger,
    IAuthenticationService service,
    IConfiguration configuration,
    IProfileService profileService) : ControllerBase {
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
    public IResult ChallengeSomething(string provider) {
        logger.LogInformation("Challenging {provider}", provider);

        var schema = provider switch {
            "GitHub" => OpenIddictClientWebIntegrationConstants.Providers.GitHub,
            "Google" => OpenIddictClientWebIntegrationConstants.Providers.Google,
            _ => throw new Exception("Unknown provider"),
        };

        return Results.Challenge(
            properties: new AuthenticationProperties
                { RedirectUri = configuration["front-end:login-success"] + "/" + provider },
            authenticationSchemes: [schema]);
    }

    [AllowAnonymous]
    [HttpGet("callback/login/github")]
    public async Task<IActionResult> GithubCallbackLogin() {
        logger.LogInformation("[Github] callback login");
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientWebIntegrationConstants.Providers.GitHub);

        var email = result.Principal!.GetClaim(ClaimTypes.Email);
        var name = result.Principal!.GetClaim(ClaimTypes.Name);
        var picture = result.Principal!.GetClaim("avatar_url");

        if (email is null || name is null) {
            return BadRequest("Could not retrieve user information from GitHub.");
        }

        var loginResponse = await service.LoginWithProvider(email, name, "GitHub");
        logger.LogInformation("Logged in with user {user}", loginResponse.User.UserName);

        service.SetTokenCookie(HttpContext, loginResponse.Token.Token, loginResponse.Token.RefreshToken);

        var redirectUri = configuration["front-end:home"]!;
        if (loginResponse.IsNewUser) {
            logger.LogInformation("[New login]: {username}", name);
            redirectUri = configuration["front-end:create-profile"]!;
            if (picture is not null) {
                await profileService.UploadProfilePictureFromUrl(loginResponse.User.Id, picture);
            }
        }

        logger.LogInformation("Redirecting to {redirectUri}", redirectUri);

        return Redirect(redirectUri);
    }

    [AllowAnonymous]
    [HttpGet("callback/login/google")]
    public async Task<IActionResult> GoogleCallbackLogin() {
        logger.LogInformation("[Google] callback login");
        var result = await HttpContext.AuthenticateAsync(OpenIddictClientWebIntegrationConstants.Providers.Google);

        var email = result.Principal!.GetClaim("email");
        var name = result.Principal!.GetClaim("given_name");
        var picture = result.Principal!.GetClaim("picture");

        if (email is null || name is null) {
            return BadRequest("Could not retrieve user information from Google.");
        }

        var loginResponse = await service.LoginWithProvider(email, name, "Google");
        logger.LogInformation("Logged in with user {user}", loginResponse.User.UserName);


        service.SetTokenCookie(HttpContext, loginResponse.Token.Token, loginResponse.Token.RefreshToken);

        var redirectUri = configuration["front-end:home"]!;

        if (loginResponse.IsNewUser) {
            redirectUri = configuration["front-end:create-profile"]!;
            if (picture is not null) {
                await profileService.UploadProfilePictureFromUrl(loginResponse.User.Id, picture);
            }
        }

        logger.LogInformation("Redirecting to {redirectUri}", redirectUri);

        return Redirect(redirectUri);
    }

    [Authorize]
    [HttpGet("whoami")]
    public IActionResult WhoAmI() {
        var user = service.GetUserWithRolesFromClaimsPrincipal(User);
        if (user is null) {
            logger.LogWarning(
                "WhoAmI: User is authorized, but claims (Id, Name, or Email) could not be retrieved from token.");
            return Problem("Could not retrieve complete user information from token.");
        }

        return Ok(user);
    }
}