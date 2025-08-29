using Microsoft.AspNetCore.Identity;

namespace Angular_Auth.Models;

public class Role(string roleName) : IdentityRole(roleName);