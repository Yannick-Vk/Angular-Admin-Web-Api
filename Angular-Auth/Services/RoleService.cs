using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Services;

public class RoleService(RoleManager<IdentityRole> manager, UserManager<User> userManager) : IRoleService {
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
        
        var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        if (user is null)  throw new ArgumentException($"user with name `{userName}` doesn't exist`");
        
        await userManager.AddToRoleAsync(user, roleName);
    }
}