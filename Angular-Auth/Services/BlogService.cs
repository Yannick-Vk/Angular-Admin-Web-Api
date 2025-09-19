using System.Text;
using Angular_Auth.Dto;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Angular_Auth.Repositories;

namespace Angular_Auth.Services;

public class BlogService(ILogger<BlogService> logger, BlogRepository repo, IUserService userService) : IBlogService {
    public async Task<IEnumerable<BlogWithFile>> GetAllBlogs() {
        var blogs = await repo.GetAllBlogs();
        return await GetBlogsWithFile(blogs);
    }

    public async Task<BlogWithFile?> GetBlog(string id) {
        var success = Guid.TryParse(id, out var guid);
        if (!success) return null;

        var blog = await repo.GetBlog(guid);
        if (blog is null) return null;

        return await GetBlogWithContent(blog);
    }

    public async Task<Guid?> UploadBlog(BlogUpload blogUpload) {
        var author = await userService.GetFullUser(blogUpload.Author);
        if (author is null) return null;

        var blog = new Blog(blogUpload, author);

        await repo.SaveBlog(blog);
        await SaveBlog(blog.Id, blogUpload.File);
        return blog.Id;
    }

    public async Task<BlogWithFile> UpdateBlog(BlogUpdate dto, UserDto loggedInUser) {
        var blog = await repo.GetBlog(dto.Id);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {dto.Id} was not found.");

        var author = loggedInUser.Username;

        if (blog.Author.UserName != author) {
            logger.LogError("User {user} attempted to update a blog belonging to {author}", author, blog.Author.UserName);
            throw new NotBlogAuthorException("You do not have permission to update this blog.");
        }

        blog.Title = dto.Title ?? blog.Title;
        blog.Description = dto.Description ?? blog.Description;
        blog.UpdatedAt = DateTime.Now;

        var updatedBlog = await repo.UpdateBlog(blog);

        if (dto.BlogContent is not null) await SaveBlog(updatedBlog.Id, dto.BlogContent);

        return await GetBlogWithContent(updatedBlog);
    }

    public async Task<Blog?> DeleteBlog(string id) {
        var success = Guid.TryParse(id, out var guid);
        if (!success) return null;

        var blog = await repo.GetBlog(guid);
        if (blog is null) return null;

        blog = await repo.DeleteBlog(blog);
        // Delete blog file
        if (blog is not null) DeleteFile(blog.Id.ToString());
        return blog;
    }

    public async Task<IEnumerable<BlogWithAuthor>> GetBlogsWithAuthor(string username) {
        return await repo.GetAllBlogsWithAuthor(username);
    }

    public async Task<IEnumerable<BlogWithFile>> SearchBlog(string searchText) {
        var blogs = await repo.FindBlogs(searchText);
        return await GetBlogsWithFile(blogs);
    }

    /// <summary>
    ///     Returns a BlogWithFile object from a given Blog object
    /// </summary>
    /// <param name="blog"></param>
    /// <returns>A blog with file content</returns>
    private static async Task<BlogWithFile> GetBlogWithContent(Blog blog) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        var uniqueFileName = blog.Id + ".md";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Initialize an empty string, if the file exist add the contents
        var content = string.Empty;
        if (File.Exists(filePath)) content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

        var newBlog = new BlogWithFile(blog, content);
        return newBlog;
    }


    private static async Task SaveBlog(Guid id, string fileContent) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        // Create the directory when it does not exist
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = id + ".md";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        var fileBytes = Encoding.UTF8.GetBytes(fileContent);

        // Save the file.
        await using var stream = new FileStream(filePath, FileMode.Create);
        await stream.WriteAsync(fileBytes);
    }

    private async Task<IEnumerable<BlogWithFile>> GetBlogsWithFile(IEnumerable<Blog> blogs) {
        var blogsWithFile = new List<BlogWithFile>();

        foreach (var blog in blogs) {
            var blogContent = await GetBlogWithContent(blog);
            blogsWithFile.Add(blogContent);
        }

        return blogsWithFile;
    }

    private void DeleteFile(string id) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        var uniqueFileName = id + ".md";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        File.Delete(filePath);
    }
}