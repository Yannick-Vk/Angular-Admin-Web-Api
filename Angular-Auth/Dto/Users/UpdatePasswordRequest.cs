namespace Angular_Auth.Dto;

public class UpdatePasswordRequest {
    public required string NewPassword { get; set; }

    public required string Password { get; set; }
}