namespace Angular_Auth.Dto.Users;

public class UpdatePasswordRequest {
    public required string NewPassword { get; set; }

    public required string Password { get; set; }
}