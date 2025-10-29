using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Angular_Auth.Dto.Auth;
using Angular_Auth.Dto.Users;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Angular_Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Angular_Auth.Utils;

namespace Angular_Auth.Services;

public class AuthenticationService(
    ILogger<AuthenticationService> logger,
    UserManager<User> userManager,
    IConfiguration configuration,
    IMailService mailService,
    ILoggerFactory loggerFactory)
    : IAuthenticationService {
    private readonly ILogger<MailBuilder> _mailBuilderLogger = loggerFactory.CreateLogger<MailBuilder>();

    private DateTime TokenExpiry() {
        return DateTime.UtcNow.AddMinutes(5);
    }

    private string GenerateRefreshToken() {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<LoginResponseWithToken> Login(LoginRequest request) {
        if (request.Username is null || request.Password is null)
            throw new CredentialsRequiredException("Username and Password are required.");

        var user = await userManager.FindByNameAsync(request.Username) ??
                   await userManager.FindByEmailAsync(request.Username);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new WrongCredentialsException("Username and/or password are incorrect.");

        if (user is not null && !user.EmailConfirmed)
            throw new EmailNotVerifiedException("Please verify your email before logging in.");

        return await CreateLoginResponseWithTokenAsync(user!);
    }

    public async Task<LoginResponseWithToken> RefreshToken(string? refreshToken) {
        if (refreshToken is null) throw new CredentialsRequiredException("Refresh token is required.");

        var user = userManager.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);

        if (user is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new WrongCredentialsException("Invalid or expired refresh token.");

        return await CreateLoginResponseWithTokenAsync(user);
    }

    public async Task<LoginResponseWithToken> Register(RegisterRequest request) {
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
            EmailConfirmed = false,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new RegistrationFailedException(
                $"Unable to register user {request.Username}:{Environment.NewLine}{ShowErrorsText(result.Errors)}");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = System.Net.WebUtility.UrlEncode(token);

        // TODO: Remove debug print
        var verificationLink = $"https://localhost:7134/api/v1/auth/verify-email?userId={user.Id}&token={encodedToken}";
        logger.LogInformation("DEBUG: User verification token: \n {link}", verificationLink);

        var mail = new MailBuilder(_mailBuilderLogger)
            .To((user.UserName, user.Email))
            .From(("JS-Blogger", "no-reply@js-blogger.be"))
            .Subject("Verify your email")
            .AddFiles("verify-email", [("link", verificationLink), ("user", request.Username)])
            .Build();
        
        // TODO: Add better email provider
        await mailService.SendEmail(mail);

        return new LoginResponseWithToken(user.Id, string.Empty, user.UserName, user.Email, DateTime.MinValue,
            string.Empty);
    }

    private async Task<LoginResponseWithToken> CreateLoginResponseWithTokenAsync(User user) {
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
        var refreshToken = GenerateRefreshToken();

        _ = int.TryParse(configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);

        await userManager.UpdateAsync(user);

        return new LoginResponseWithToken(user.Id, jwt, user.UserName!, user.Email!, TokenExpiry(), refreshToken);
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

    public UserWithRoles? GetUserWithRolesFromClaimsPrincipal(ClaimsPrincipal claims) {
        var userId = claims.FindFirst("Id")?.Value;
        var userName = claims.FindFirst("Username")?.Value;
        var userEmail = claims.FindFirst("Email")?.Value;

        if (userId is null || userName is null || userEmail is null) return null;

        var userRoles = claims.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList();

        return new UserWithRoles(new UserDto(userId, userName, userEmail), userRoles);
    }

    public UserDto? GetUserFromClaimsPrincipal(ClaimsPrincipal claims) {
        var userId = claims.FindFirst("Id")?.Value;
        var userName = claims.FindFirst("Username")?.Value;
        var userEmail = claims.FindFirst("Email")?.Value;

        if (userId is null || userName is null || userEmail is null) return null;

        return new UserDto(userId, userName, userEmail);
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

    public void SetTokenCookie(HttpContext context, string token, string refreshToken) {
        context.Response.Cookies.Append("accessToken", token,
            new CookieOptions {
                Expires = TokenExpiry(),
                HttpOnly = true, // Set as Http-only cookie
                IsEssential = true, // Cookie is required for the app to work
                Secure = true, // Via Https or SSL only
                SameSite = SameSiteMode.None,
            });
        context.Response.Cookies.Append("refreshToken", refreshToken,
            new CookieOptions {
                Expires = DateTime.UtcNow.AddDays(int.Parse(configuration["JWT:RefreshTokenValidityInDays"] ?? "1")),
                HttpOnly = true, // Set as Http-only cookie
                IsEssential = true, // Cookie is required for the app to work
                Secure = true, // Via Https or SSL only
                SameSite = SameSiteMode.None,
            });
    }

    public void RemoveTokenCookie(HttpContext context) {
        context.Response.Cookies.Append("accessToken", "",
            new CookieOptions {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
            });
        context.Response.Cookies.Append("refreshToken", "",
            new CookieOptions {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
            });
    }

    public async Task<bool> VerifyEmail(string userId, string token) {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null) {
            logger.LogError("VerifyEmail: User with ID {UserId} not found.", userId);
            return false;
        }

        logger.LogInformation("VerifyEmail: Found user {Email}. Attempting to confirm email with token {Token}",
            user.Email, token);

        // The token from the URL route is automatically decoded by ASP.NET Core's model binding.
        // Manually decoding it again would corrupt it.
        var result = await userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded) {
            logger.LogError("VerifyEmail: Email confirmation failed for user {Email}. Errors: {Errors}", user.Email,
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return result.Succeeded;
    }

    public async Task<LoginResponseWithProvider> LoginWithProvider(string email, string name, string provider) {
        var user = await userManager.FindByEmailAsync(email);
        bool isNewUser = false;

        if (user is null) {
            logger.LogInformation("Creating new user");
            // To prevent username collision
            var existingUserWithSameName = await userManager.FindByNameAsync(name);
            if (existingUserWithSameName != null) {
                name = $"{name}#{new Random().Next(1000, 9999)}";
            }

            user = new User {
                UserName = name,
                Email = email,
                EmailConfirmed = true // Email is considered verified from an external provider
            };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) {
                throw new RegistrationFailedException(
                    $"Unable to create user {name}. Errors: {ShowErrorsText(result.Errors)}");
            }

            await userManager.AddLoginAsync(user, new UserLoginInfo(provider, email, provider));
            isNewUser = true;
        }
        else {
            var logins = await userManager.GetLoginsAsync(user);
            if (logins.All(l => l.LoginProvider != provider)) {
                await userManager.AddLoginAsync(user, new UserLoginInfo(provider, email, provider));
            }
        }

        var loginResponse = await CreateLoginResponseWithTokenAsync(user);
        return new LoginResponseWithProvider {
            User = user,
            Token = loginResponse,
            IsNewUser = isNewUser
        };
    }
}