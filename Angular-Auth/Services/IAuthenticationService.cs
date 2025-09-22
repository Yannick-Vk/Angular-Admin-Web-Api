using System.IdentityModel.Tokens.Jwt;
using Angular_Auth.Dto;
using Microsoft.IdentityModel.Tokens;

namespace Angular_Auth.Services;

public interface IAuthenticationService {
    Task<string> Login(LoginRequest request);
    Task<string> Register(RegisterRequest request);
    public UserDto? GetUserFromRequest(HttpRequest req);
    public UserWithRoles? GetUserWithRolesFromRequest(HttpRequest req);
    public JwtSecurityToken? GetSecurityTokenFromRequest(HttpRequest req);
}