using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Angular_Auth.Dto;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Angular_Auth.Services;

public class AuthenticationService(
    ILogger<AuthenticationService> logger,
    UserManager<User> userManager,
    IConfiguration configuration)
    : IAuthenticationService {

    private DateTime TokenExpiry() {
        return DateTime.Now.AddMinutes(30);
    }
    
    public async Task<string> Login(LoginRequest request) {
        if (request.Username is null || request.Password is null)
            throw new CredentialsRequiredException("Username and Password are required.");

        var user = await userManager.FindByNameAsync(request.Username) ??
                   await userManager.FindByEmailAsync(request.Username);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new WrongCredentialsException("Username and/or password are incorrect.");

        var userRoles = await userManager.GetRolesAsync(user);
        var authClaims = new List<Claim> {
            new("Id", user.Id),
            new("Username", user.UserName ?? string.Empty),
            new("Email", user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var token = GetToken(authClaims);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public async Task<string> Register(RegisterRequest request) {
        if (request.Email is null || request.Username is null || request.Password is null)
            throw new CredentialsRequiredException("Email, Username and Password are required.");

        // Find a user that already has a given Email or Username
        var userByEmail = await userManager.FindByEmailAsync(request.Email);
        var userByUsername = await userManager.FindByNameAsync(request.Username);
        if (userByEmail is not null || userByUsername is not null)
            throw new UserAlreadyExistsException(
                $"User with email {request.Email} or username {request.Username} already exists.");

        // Create a new user
        User user = new() {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Username,
            Email = request.Email,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new RegistrationFailedException(
                $"Unable to register user {request.Username}:{Environment.NewLine}{ShowErrorsText(result.Errors)}");

        return await Login(new LoginRequest { Username = request.Email, Password = request.Password, });
    }

    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims) {
        var secret = configuration["JWT:Secret"];
        if (secret is null) throw new ArgumentException("JWT:Secret is not configured.");

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        
        var token = new JwtSecurityToken(
            configuration["JWT:ValidIssuer"],
            configuration["JWT:ValidAudience"],
            expires: TokenExpiry(),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }

    private static string ShowErrorsText(IEnumerable<IdentityError> errors) {
        return string.Join(Environment.NewLine, errors.Select(error => error.Description).ToArray());
    }

    public UserDto? GetUserFromRequest(HttpRequest req) {
        var token = GetSecurityTokenFromRequest(req);
        if (token is null) return null;

        var userId = token.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
        var userName = token.Claims.FirstOrDefault(x => x.Type == "Username")?.Value;
        var userEmail = token.Claims.FirstOrDefault(x => x.Type == "Email")?.Value;

        if (userId is null || userName is null || userEmail is null) return null;

        return new UserDto(userId, userName, userEmail);
    }

    public UserWithRoles? GetUserWithRolesFromRequest(HttpRequest req) {
        var jwtToken = GetSecurityTokenFromRequest(req);
        if (jwtToken is null) return null;

        var user = GetUserFromRequest(req);
        if (user is null) return null;

        var userRoles = jwtToken.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value).ToList();

        return new UserWithRoles(user, userRoles);
    }

    public JwtSecurityToken? GetSecurityTokenFromRequest(HttpRequest req) {
        string? authHeader = req.Headers.Authorization;
        if (authHeader is null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) {
            logger.LogInformation("No JWT token found in the request or incorrect scheme.");
            return null;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = configuration["JWT:Secret"];
        if (secret is null) {
            logger.LogError("JWT:Secret is not configured.");
            return null;
        }

        var issuer = configuration["JWT:ValidIssuer"];
        var audience = configuration["JWT:ValidAudience"];
        if (issuer is null || audience is null) {
            logger.LogError("JWT:ValidIssuer or JWT:ValidAudience is not configured.");
            return null;
        }

        var key = Encoding.UTF8.GetBytes(secret);

        try {
            tokenHandler.ValidateToken(token, new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return (JwtSecurityToken)validatedToken;
        }
        catch (Exception e) {
            logger.LogError(e, "Error validating JWT token.");
        }

        return null;
    }

    public void SetTokenCookie(HttpContext context, string token) {
        context.Response.Cookies.Append("accessToken", token,
            new CookieOptions {
                Expires = TokenExpiry(),
                HttpOnly = true, // Set as Http-only cookie
                IsEssential = true, // Cookie is required for the app to work
                Secure = true, // Via Https or SSL only
                SameSite = SameSiteMode.None,
            });
    }
}