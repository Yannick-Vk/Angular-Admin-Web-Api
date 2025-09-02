namespace Angular_Auth.Dto;

public class LoginResponse {
    
    // On valid Response
    public bool Success { get; private set; }
    public string? Token { get; init; }
    public DateTime? Expiration { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
    
    public LoginResponse(string token, DateTime expiration, string username, string email) {
        Success = true;
        Token = token;
        Expiration = expiration;
        Username = username;
        Email = email;
    }
    
    // On invalid Response
    public string? Message { get; private set; }

    public LoginResponse(string message) {
        Success = false;
        Message = message;
    }

    
}
