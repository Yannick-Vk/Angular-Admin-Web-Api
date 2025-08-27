namespace Angular_Auth.Dto;

public class ReturnToken {
    public required string Token { get; init; }
    public DateTime Expiration { get; init; }
}