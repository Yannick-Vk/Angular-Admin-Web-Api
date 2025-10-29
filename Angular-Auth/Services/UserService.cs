using Angular_Auth.Dto.Users;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Angular_Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Services;

public class UserService(UserManager<User> manager, IProfileService profileService) : IUserService {
    public async Task<List<UserDto>> GetUsers() {
        var users = await manager.Users
            .Select(user => new UserDto(user))
            .ToListAsync();
        return users;
    }

    public async Task<User> GetUserById(string userId) {
        var user = await manager.Users.FirstAsync(user => user.Id == userId);
        return user ?? throw new UserNotFoundException($"Cannot find user with id {userId}");
    }

    public async Task<User> GetUserByUsername(string username) {
        var user = await manager.Users.FirstOrDefaultAsync(u => u.UserName == username);
        return user ?? throw new UserNotFoundException($"Cannot find user with username {username}");
    }

    public async Task<User?> GetFullUser(string id) {
        return await manager.FindByIdAsync(id);
    }

    public async Task<byte[]> GetUserProfilePicture(string userId) {
        return await profileService.GetProfilePicture(userId);
    }
}