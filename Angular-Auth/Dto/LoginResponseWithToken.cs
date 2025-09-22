namespace Angular_Auth.Dto;

public class LoginResponseWithToken(string token, string userName, DateTime expiry) {
    public string Token { get; set; } = token;
    public string UserName { get; set; } = userName;
    public DateTime Expiry { get; set; } = expiry;

    public LoginResponse RemoveToken() {
        return new LoginResponse(this.UserName, this.Expiry);
    }
}