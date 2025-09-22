namespace Angular_Auth.Dto;

public class LoginResponse(string username, DateTime expiration) {
    public string Username { get; set; } = username;
    public DateTime Expiry { get; set; } = expiration;

    public static LoginResponse FromResponseWithToken(LoginResponseWithToken response) {
        return new LoginResponse(response.UserName, response.Expiry);
    }
}