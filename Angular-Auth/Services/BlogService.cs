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

    public async Task<Guid> UploadBlog(UserDto user, BlogUpload blogUpload) {
        var author = await userService.GetFullUser(user.Username);
        if (author is null) throw new UnauthorizedAccessException($"Cannot find user {user.Username}");

        var blog = new Blog(blogUpload, author);

        await repo.SaveBlog(blog);
        await SaveBlogFile(blog.Id, blogUpload.File);
        return blog.Id;
    }

    public async Task<BlogWithFile> UpdateBlog(BlogUpdate dto, UserDto loggedInUser) {
        var blog = await repo.GetBlog(dto.Id);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {dto.Id} was not found.");

        var author = await userService.GetFullUser(loggedInUser.Username);
        if (author is null) throw new UnauthorizedAccessException("Could not find user");

        if (!UserIsAuthor(blog, author)) {
            logger.LogError("User {user} attempted to updated a blog without being its author.", author.UserName);
            throw new NotBlogAuthorException("You do not have permission to update this blog.");
        }

        blog.Title = dto.Title ?? blog.Title;
        blog.Description = dto.Description ?? blog.Description;
        blog.UpdatedAt = DateTime.Now;

        var updatedBlog = await repo.UpdateBlog(blog);

        if (dto.BlogContent is not null) await SaveBlogFile(updatedBlog.Id, dto.BlogContent);

        return await GetBlogWithContent(updatedBlog);
    }

    public async Task<Blog> DeleteBlog(string id, UserDto user) {
        var success = Guid.TryParse(id, out var guid);
        if (!success) throw new BlogNotFoundException($"Blog with ID {id} was not found.");

        var blog = await repo.GetBlog(guid);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {id} was not found.");

        var fullUser = await userService.GetFullUser(user.Username);
        if (fullUser is null) throw new UnauthorizedAccessException("Could not find user");

        if (!UserIsAuthor(blog, fullUser))
            throw new NotBlogAuthorException("You do not have permission to delete this blog.");

        await repo.DeleteBlog(blog);
        // Delete blog file
        DeleteBlogFile(blog.Id.ToString());
        return blog;
    }

    public async Task<IEnumerable<BlogWithAuthor>> GetBlogsWithAuthor(string username) {
        return await repo.GetAllBlogsWithAuthor(username);
    }

    public async Task<IEnumerable<BlogWithFile>> SearchBlog(string searchText) {
        var blogs = await repo.FindBlogs(searchText);
        return await GetBlogsWithFile(blogs);
    }

    // Find user and blog, then add user to authors and send an update
    public async Task AddAuthor(string blogId, string userId, UserDto loggedInUser) {
        var success = Guid.TryParse(blogId, out var guid);
        if (!success) return;

        var blog = await repo.GetBlog(guid);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var loggedInUserFull = await userService.GetFullUser(loggedInUser.Username);
        if (loggedInUserFull is null) throw new UnauthorizedAccessException("Could not find logged in user");

        if (!UserIsAuthor(blog, loggedInUserFull)) {
            throw new NotBlogAuthorException("You do not have permission to add an author to this blog.");
        }

        var user = await userService.GetFullUser(userId);
        if (user is null) return;

        if (UserIsAuthor(blog, user)) {
            return;
        }

        blog.Authors.Add(user);
        await repo.UpdateBlog(blog);
    }

    /// <summary>
    ///     Returns a BlogWithFile object from a given Blog object
    /// </summary>
    /// <param name="blog"></param>
    /// <returns>A blog with file content</returns>
    private static async Task<BlogWithFile> GetBlogWithContent(Blog blog) {
        var content = await GetFileContent(blog.Id.ToString(), "blogs", ".md");
        var newBlog = new BlogWithFile(blog, content);
        return newBlog;
    }

    private static async Task SaveBlogFile(Guid id, string fileContent) {
        await SaveFile(id.ToString(), fileContent, "blogs", ".md");
    }

    private static async Task<string> GetFileContent(string filename, string folder, string extension) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"uploads/{folder}");
        var uniqueFileName = filename + extension;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Initialize an empty string, if the file exist add the contents
        var content = string.Empty;
        if (File.Exists(filePath)) content = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        return content;
    }


    private static async Task SaveFile(string filename, string fileContent, string folder, string ext) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"uploads/{folder}");
        // Create the directory when it does not exist
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = filename + ext;
        var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

        var fileBytes = Encoding.UTF8.GetBytes(fileContent);

        // Save the file.
        await using var stream = new FileStream(fullPath, FileMode.Create);
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

    public static void DeleteBlogFile(string blogId) {
        DeleteFile(blogId, "blogs", ".md");
    }

    private static void DeleteFile(string filename, string folder, string extension) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads/{folder}");
        var uniqueFileName = filename + extension;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
        File.Delete(filePath);
    }

    private static bool UserIsAuthor(Blog blog, User user) {
        return blog.Authors.Exists(author => author.Id == user.Id);
    }
}