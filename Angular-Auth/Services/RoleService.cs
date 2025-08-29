using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Services;

public class RoleService(RoleManager<IdentityRole> manager) : IRoleService {
    public async Task AddRole(string roleName) {
        await manager.CreateAsync(new Role { Name = roleName });
    }

    public async Task<IEnumerable<IdentityRole>> GetAllRoles() {
        return await manager.Roles
            .AsNoTracking()
            .ToListAsync();
    }
}