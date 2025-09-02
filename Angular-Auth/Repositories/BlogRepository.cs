using Angular_Auth.Models;

namespace Angular_Auth.Repositories;

public class BlogRepository(AppDbContext context)
{
    public async Task SaveBlog(Blog blog)
    {
        context.Blogs.Add(blog);
        await context.SaveChangesAsync();
    }
}