namespace Angular_Auth.Dto;

public class LoginResponse {
    
    // On valid Response
    public bool Success { get; init; }
    public string? Token { get; init; }
    
    // On invalid Response
    public string? Message { get; init; }
}
