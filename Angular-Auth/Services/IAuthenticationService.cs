using System.IdentityModel.Tokens.Jwt;
using Angular_Auth.Dto;
using Microsoft.IdentityModel.Tokens;

namespace Angular_Auth.Services;

public interface IAuthenticationService {
    Task<LoginResponseWithToken> Login(LoginRequest request);
    Task<LoginResponseWithToken> Register(RegisterRequest request);
    public UserDto? GetUserFromRequest(HttpRequest req);
    public UserWithRoles? GetUserWithRolesFromRequest(HttpRequest req);
    public JwtSecurityToken? GetSecurityTokenFromRequest(HttpRequest req);
    public void SetTokenCookie(HttpContext context, string token);
}