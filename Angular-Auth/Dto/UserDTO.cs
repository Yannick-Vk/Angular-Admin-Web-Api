using System.IdentityModel.Tokens.Jwt;
using Angular_Auth.Models;

namespace Angular_Auth.Dto;

public class UserDto {

    public UserDto(string userId, string userName, string userEmail) {
        Id = userId;
        Username = userName;
        Email = userEmail;
    }

    public UserDto(User user) {
        Id = user.Id;
        Username = user.UserName?? throw new ArgumentException("Username is required");
        Email = user.Email?? throw new ArgumentException("Email is required");
    }

    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}