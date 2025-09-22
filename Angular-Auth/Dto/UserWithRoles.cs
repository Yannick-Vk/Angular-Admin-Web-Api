using Angular_Auth.Models;

namespace Angular_Auth.Dto;

public class UserWithRoles {
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    
    public List<string> Roles { get; set; }

    public UserWithRoles(UserDto user, List<string> roles) {
        Id = user.Id;
        Username = user.Username;
        Email = user.Email;
        Roles = roles;
    }
}