using Angular_Auth.Dto;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Services;

public class RoleService(ILogger<RoleService> logger, RoleManager<IdentityRole> manager, UserManager<User> userManager) : IRoleService {

    public async Task<User> GetUser(string userName) {
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        return user ?? throw new ArgumentException($"user with name `{userName}` doesn't exist`");
    }
    
    public async Task AddRole(Role role) {
        await manager.CreateAsync(role);
    }

    public async Task<IEnumerable<IdentityRole>> GetAllRoles() {
        return await manager.Roles
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IdentityResult> DeleteRole(string roleName) {
        var role = await manager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role is null) return IdentityResult.Failed(new IdentityError {
            Code = "Not found",
            Description = $"Cannot find role with the given name `{roleName}`"
        });
        var result = await manager.DeleteAsync(role);
        return result;
    }

    public async Task AddRoleToUser(string roleName, string userName) {
        var role = await manager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role is null) throw new ArgumentException($"role with name `{roleName}` doesn't exist`");
        
        var user = await GetUser(userName);
        
        logger.LogInformation("Adding role `{RoleName}` to user `{UserName}`", roleName, userName);
        
        await userManager.AddToRoleAsync(user, roleName);
    }

    public async Task RemoveRoleFromUser(string roleName, string userName) {
        var user = await GetUser(userName);
        
        await userManager.RemoveFromRoleAsync(user, roleName);
    }

    public async Task<IEnumerable<UserDto>> GetUsersWithRole(string roleName) {
        return (await userManager.GetUsersInRoleAsync(roleName)).Select(user => new UserDto(user));
    }

    public async Task<bool> UserHasRole(string roleName, string userName) {
        var user = await GetUser(userName);
        return await userManager.IsInRoleAsync(user, roleName);
    }
}