using Angular_Auth.Dto.Blogs;
using Angular_Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Angular_Auth.Repositories;

public class BlogRepository(AppDbContext context) {
    public async Task SaveBlog(Blog blog) {
        context.Blogs.Add(blog);
        await context.SaveChangesAsync();
    }

    public async Task<List<Blog>> GetAllBlogs() {
        return await context.Blogs
            .Include(b => b.Authors)
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Blog?> GetBlog(Guid guid) {
        return await context.Blogs
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == guid);
    }

    public async Task<Blog> UpdateBlog(Blog blog) {
        context.Blogs.Update(blog);
        await context.SaveChangesAsync();
        return blog;
    }

    public async Task DeleteBlog(Blog blog) {
        context.Blogs.Remove(blog);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<BlogWithAuthor>> GetAllBlogsWithAuthor(User author) {
        return await context.Blogs
            .Include(b => b.Authors)
            .Where(b => b.Authors.Contains(author))
            .OrderByDescending(b => b.CreatedAt)
            .Select(blog => new BlogWithAuthor(blog))
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Blog>> FindBlogs(string searchWord) {
        searchWord = searchWord.Trim().ToLower();

        return await context.Blogs
                .Include(b => b.Authors)
                .Where(b => b.Title.ToLower().Contains(searchWord))
                .OrderByDescending(b => b.CreatedAt)
                .AsNoTracking()
                .ToListAsync()
            ;
    }
}