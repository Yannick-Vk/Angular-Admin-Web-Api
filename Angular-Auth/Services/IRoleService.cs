using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Services;

public interface IRoleService {
    Task AddRole(string roleName);
    Task<IEnumerable<IdentityRole>> GetAllRoles();
}