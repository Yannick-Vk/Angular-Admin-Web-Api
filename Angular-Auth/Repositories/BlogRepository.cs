using Angular_Auth.Dto;
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

    public async Task<Blog?> GetBlog(string id) {
        var success = Guid.TryParse(id, out var guid);
        if (!success) return null;
        return await context.Blogs.FindAsync(guid);
    }

    public async Task<Blog?> UpdateBlog(BlogUpdate updatedBlog) {
        var blog = await context.Blogs.FindAsync(updatedBlog.Id);
        if (blog is null) return null;
        
        context.Blogs.Update(blog);
        await context.SaveChangesAsync();
        return blog;
    }
}