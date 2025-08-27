namespace Angular_Auth.Dto;

public class UserDto {
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}