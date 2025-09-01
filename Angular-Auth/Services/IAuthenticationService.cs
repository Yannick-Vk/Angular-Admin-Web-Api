using Angular_Auth.Dto;

namespace Angular_Auth.Services;

public interface IAuthenticationService {
    Task<LoginResponse> Login(LoginRequest request);
    Task<LoginResponse> Register(RegisterRequest request);
}