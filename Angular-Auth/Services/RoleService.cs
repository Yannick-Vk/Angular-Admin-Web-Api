using System.Text;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Services;

public class RoleService(RoleManager<IdentityRole> manager, ILogger<RoleService> logger) : IRoleService {
    public async Task AddRole(Role role) {
        await manager.CreateAsync(role);
    }

    public async Task<IEnumerable<IdentityRole>> GetAllRoles() {
        return await manager.Roles
            .AsNoTracking()
            .ToListAsync();
    }
}