using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Models;

public class User : IdentityUser {
    [MaxLength(50)]
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

    public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}