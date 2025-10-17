using Angular_Auth.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Repositories;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options) {
    public DbSet<Blog> Blogs { get; set; }
    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);

        builder.Entity<User>().Property(u => u.Id).HasMaxLength(256);

        // builder.Entity<Blog>()
        //     .HasMany(b => b.Authors)
        //     .WithMany(u => u.Blogs);
    }
}