using System.ComponentModel.DataAnnotations;

namespace Angular_Auth.Dto.Users;

public class UpdateUsernameRequest {
    public required string Username { get; set; }
}