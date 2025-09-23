namespace Angular_Auth.Dto;

public class LoginResponse(string id, string username, string email, DateTime expiration) {
    public string Id { get; set; } = id;
    public string Username { get; set; } = username;
    public string Email { get; set; } = email;
    public DateTime Expiry { get; set; } = expiration;

    public static LoginResponse FromResponseWithToken(LoginResponseWithToken response) {
        return new LoginResponse(response.Id, response.UserName, response.Email, response.Expiry);
    }
}