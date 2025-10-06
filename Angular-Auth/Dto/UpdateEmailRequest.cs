using System.ComponentModel.DataAnnotations;

namespace Angular_Auth.Dto;

public class UpdateEmailRequest {
    [EmailAddress]
    public required string Email { get; set; }
}