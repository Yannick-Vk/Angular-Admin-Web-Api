using Angular_Auth.Models;

namespace Angular_Auth.Dto.Auth;

public class LoginResponseWithProvider
{
    public required User User { get; set; }
    public required LoginResponseWithToken Token { get; set; }
    public bool IsNewUser { get; set; }
}