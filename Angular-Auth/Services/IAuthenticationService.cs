using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Angular_Auth.Dto;
using Microsoft.IdentityModel.Tokens;

namespace Angular_Auth.Services;

public interface IAuthenticationService {
    public Task<LoginResponseWithToken> Login(LoginRequest request);
    public Task<LoginResponseWithToken> Register(RegisterRequest request);
    public UserDto? GetUserFromRequest(HttpRequest req);
    public UserWithRoles? GetUserWithRolesFromRequest(HttpRequest req);
    public UserWithRoles? GetUserWithRolesFromClaimsPrincipal(ClaimsPrincipal claims);
    public JwtSecurityToken? GetSecurityTokenFromRequest(HttpRequest req);
    public void SetTokenCookie(HttpContext context, string token);
    public void RemoveTokenCookie(HttpContext context);
}