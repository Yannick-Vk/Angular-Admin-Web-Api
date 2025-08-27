using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Services;

public class UserService(UserManager<User> manager) : IUserService {
    public async Task<List<User>> GetUsers() {
        var users = await manager.Users.ToListAsync();
        return users;
    }
}