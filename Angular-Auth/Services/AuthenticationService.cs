using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Angular_Auth.Dto;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Angular_Auth.Services;

public class AuthenticationService(UserManager<User> userManager, IConfiguration configuration)
    : IAuthenticationService {
    /// <summary>
    /// Login a user via a request, username can be either a username or an email
    /// </summary>
    /// <param name="request">The request parameters</param>
    /// <returns>A json object with a JWT Token</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<string> Login(LoginRequest request) {
        if (request.Username is null || request.Password is null) {
            throw new ArgumentException("Username and Password are required.");
        }

        var user = await userManager.FindByNameAsync(request.Username) ??
                   await userManager.FindByEmailAsync(request.Username);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password)) {
            throw new ArgumentException($"Unable to authenticate user {request.Username}");
        }

        var authClaims = new List<Claim> {
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = GetToken(authClaims);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        var data = new ReturnToken {
            Token = jwt,
            Expiration = token.ValidTo,
        };
        return JsonSerializer.Serialize(data);
    }

    /// <summary>
    /// Register a user with the given params, then log them in
    /// </summary>
    /// <param name="request">The request parameters</param>
    /// <returns>A json object with a JWT Token</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<string> Register(RegisterRequest request) {
        if (request.Email is null || request.Username is null || request.Password is null) {
            throw new ArgumentException("Email, Username and Password are required.");
        }
        // Find a user that already has a given Email or Username
        var userByEmail = await userManager.FindByEmailAsync(request.Email);
        var userByUsername = await userManager.FindByNameAsync(request.Username);
        if (userByEmail is not null || userByUsername is not null) {
            throw new ArgumentException(
                $"User with email {request.Email} or username {request.Username} already exists.");
        }
        
        // Create a new user
        User user = new() {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = request.Username,
            Email = request.Email,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded) {
            throw new ArgumentException(
                $"Unable to register user {request.Username} errors: {ShowErrorsText(result.Errors)}");
        }

        return await Login(new LoginRequest { Username = request.Email, Password = request.Password });
    }

    /// <summary>
    /// Create a token with the given claims
    /// </summary>
    /// <param name="authClaims">The claims of the token</param>
    /// <returns>A jwt token</returns>
    /// <exception cref="ArgumentException"></exception>
    private JwtSecurityToken GetToken(IEnumerable<Claim> authClaims) {
        var secret = configuration["JWT:Secret"];
        if (secret is null) {
            throw new ArgumentException("JWT:Secret is not configured.");
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddMinutes(30),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }

    private static string ShowErrorsText(IEnumerable<IdentityError> errors) {
        return string.Join(", ", errors.Select(error => error.Description).ToArray());
    }
}