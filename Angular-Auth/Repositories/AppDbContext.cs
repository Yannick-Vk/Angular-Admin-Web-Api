using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Repositories;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options) { }