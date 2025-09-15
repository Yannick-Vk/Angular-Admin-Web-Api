using Angular_Auth.Models;

namespace Angular_Auth.Dto;

public class UserDto {
    public UserDto() { }

    public UserDto(User user) {
        Id = user.Id;
        Username = user.UserName;
        Email = user.Email;
    }

    public string? Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
}