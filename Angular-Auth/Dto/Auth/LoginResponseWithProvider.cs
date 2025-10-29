using Angular_Auth.Models;

namespace Angular_Auth.Dto.Auth;

public class LoginResponseWithProvider
{
    public User User { get; set; }
    public LoginResponseWithToken Token { get; set; }
    public bool IsNewUser { get; set; }
}