using Angular_Auth.Dto;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Services;

public interface IRoleService {
    Task AddRole(Role role);
    Task<IEnumerable<IdentityRole>> GetAllRoles();
    Task<IdentityResult> DeleteRole(string roleName);
    Task AddRoleToUser(string roleName, string userName);
    Task RemoveRoleFromUser(string roleName, string userName);
    Task<IEnumerable<UserDto>> GetUsersWithRole(string roleName);
    Task<bool> UserHasRole(string roleName, string userName);
}