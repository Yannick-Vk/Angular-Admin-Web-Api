using Angular_Auth.Dto;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Services;

public class UserService(UserManager<User> manager) : IUserService {
    public async Task<List<UserDto>> GetUsers() {
        var users = await manager.Users
            .Select(user => new UserDto(user))
            .ToListAsync();
        return users;
    }

    public async Task<UserDto?> GetUserDto(string username) {
        var user = await manager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        return user is null ? null : new UserDto(user);
    }

    public async Task<User?> GetFullUser(string username) {
        return await manager.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }
}