using Angular_Auth.Dto;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Services;

public class UserService(UserManager<User> manager) : IUserService {
    public async Task<List<UserDto>> GetUsers() {
        var users = await manager.Users.Select(user => new UserDto {
            Id = user.Id.ToString(),
            Username = user.UserName,
            Email = user.Email
        }).ToListAsync();
        return users;
    }
}