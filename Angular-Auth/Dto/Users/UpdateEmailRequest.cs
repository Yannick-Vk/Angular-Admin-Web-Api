using System.ComponentModel.DataAnnotations;

namespace Angular_Auth.Dto.Users;

public class UpdateEmailRequest {
    [EmailAddress] public required string Email { get; set; }
}