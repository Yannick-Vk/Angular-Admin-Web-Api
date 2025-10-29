using Angular_Auth.Dto.Users;
using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Services.Interfaces;

public interface IRoleService {
    Task CreateNewRole(Role role);
    Task<IEnumerable<IdentityRole>> GetAllRoles();
    Task<IdentityResult> DeleteRole(string roleName);
    Task AddRoleToUser(string roleName, string userId);
    Task RemoveRoleFromUser(string roleName, string userId);
    Task<IEnumerable<UserDto>> GetUsersWithRole(string roleName);
    Task<bool> UserHasRole(string roleName, UserWithRoles user);
    Task<IEnumerable<string>> GetRolesFromUser(string userId);
}