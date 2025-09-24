namespace Angular_Auth.Dto;

public class LoginResponseWithToken(string id, string token, string userName, string email, DateTime expiry, string refreshToken) {
    public string Token { get; set; } = token;
    public string Id { get; set; } = id;
    public string UserName { get; set; } = userName;
    public string Email { get; set; } = email;
    public DateTime Expiry { get; set; } = expiry;
    public string RefreshToken { get; set; } = refreshToken;

    public LoginResponse RemoveToken() {
        return new LoginResponse(Id, UserName, Email, Expiry, string.Empty);
    }
}