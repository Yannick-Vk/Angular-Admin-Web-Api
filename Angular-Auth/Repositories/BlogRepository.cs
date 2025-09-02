using Angular_Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Repositories;

public class BlogRepository(AppDbContext context) {
    public async Task SaveBlog(Blog blog) {
        context.Blogs.Add(blog);
        await context.SaveChangesAsync();
    }

    public async Task<List<Blog>> GetAllBlogs() {
        return await context.Blogs.AsNoTracking().ToListAsync();
    }
}