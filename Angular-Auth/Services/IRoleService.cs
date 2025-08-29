using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Services;

public interface IRoleService {
    Task AddRole(Role role);
    Task<IEnumerable<IdentityRole>> GetAllRoles();
    Task<IdentityResult> DeleteRole(string roleName);
}