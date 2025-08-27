using Angular_Auth.Dto;

namespace Angular_Auth.Services;

public interface IAuthenticationService {
    Task<string> Login(LoginRequest request);
    Task<string> Register(RegisterRequest request);
}