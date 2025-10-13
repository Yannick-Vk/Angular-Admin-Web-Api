using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Angular_Auth.Dto.Auth;
using Angular_Auth.Dto.Users;

namespace Angular_Auth.Services.Interfaces;

public interface IAuthenticationService {
    public Task<LoginResponseWithToken> Login(LoginRequest request);
    public Task<LoginResponseWithToken> Register(RegisterRequest request);
    public Task<LoginResponseWithToken> RefreshToken(string? refreshToken);
    public UserDto? GetUserFromRequest(HttpRequest req);
    public UserWithRoles? GetUserWithRolesFromRequest(HttpRequest req);
    public UserWithRoles? GetUserWithRolesFromClaimsPrincipal(ClaimsPrincipal claims);
    public UserDto? GetUserFromClaimsPrincipal(ClaimsPrincipal claims);
    public JwtSecurityToken? GetSecurityTokenFromRequest(HttpRequest req);
    public void SetTokenCookie(HttpContext context, string token, string refreshToken);
    public void RemoveTokenCookie(HttpContext context);
}