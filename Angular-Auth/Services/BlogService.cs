using Angular_Auth.Dto.Blogs;
using Angular_Auth.Dto.Users;
using Angular_Auth.Exceptions;
using Angular_Auth.Models;
using Angular_Auth.Repositories;
using Angular_Auth.Services.Interfaces;

namespace Angular_Auth.Services;

public class BlogService(ILogger<BlogService> logger, BlogRepository repo, IUserService userService) : IBlogService {
    private static readonly FileService BlogFilesService = new("blogs", ".md");

    public async Task<IEnumerable<BlogWithContent>> GetAllBlogs() {
        var blogs = await repo.GetAllBlogs();
        var blogsWithContent = new List<BlogWithContent>();
        foreach (var blog in blogs) {
            var content = await BlogFilesService.GetFileContent(blog.Id.ToString());
            blogsWithContent.Add(new BlogWithContent(blog, content));
        }

        return blogsWithContent;
    }

    public async Task<BlogWithContent?> GetBlog(string id) {
        var success = Guid.TryParse(id, out var guid);
        if (!success) return null;

        var blog = await repo.GetBlog(guid);
        if (blog is null) return null;

        return await GetBlogWithContent(blog);
    }

    public async Task<Guid> UploadBlog(UserDto user, BlogUpload blogUpload) {
        var author = await userService.GetUserByUsername(user.Username);
        if (author is null) throw new UnauthorizedAccessException($"Cannot find user {user.Username}");

        var blog = new Blog(blogUpload, author);

        await repo.SaveBlog(blog);
        await SaveBlogFile(blog.Id, blogUpload.File);
        if (blogUpload.BannerImage is not null) await SaveBanner(blog.Id, blogUpload.BannerImage);
        return blog.Id;
    }

    public async Task<BlogWithContent> UpdateBlog(BlogUpdate dto, UserDto loggedInUser) {
        var blog = await repo.GetBlog(dto.Id);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {dto.Id} was not found.");

        var author = await userService.GetUserByUsername(loggedInUser.Username);
        if (author is null) throw new UnauthorizedAccessException("Could not find user");

        if (!UserIsAuthor(blog, author)) {
            logger.LogError("User {user} attempted to updated a blog without being its author.", author.UserName);
            throw new NotBlogAuthorException("You do not have permission to update this blog.");
        }

        blog.Title = dto.Title ?? blog.Title;
        blog.Description = dto.Description ?? blog.Description;
        blog.UpdatedAt = DateTime.UtcNow;

        var updatedBlog = await repo.UpdateBlog(blog);

        if (dto.Content is not null) await SaveBlogFile(updatedBlog.Id, dto.Content);
        if (dto.BannerImage is not null) {
            DeleteBanner(blog.Id);
            await SaveBanner(blog.Id, dto.BannerImage);
        }

        return await GetBlogWithContent(updatedBlog);
    }

    public async Task<Blog> DeleteBlog(string id, UserDto user) {
        var success = Guid.TryParse(id, out var guid);
        if (!success) throw new BlogNotFoundException($"Blog with ID {id} was not found.");

        var blog = await repo.GetBlog(guid);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {id} was not found.");

        var fullUser = await userService.GetUserByUsername(user.Username);
        if (fullUser is null) throw new UnauthorizedAccessException("Could not find user");

        if (!UserIsAuthor(blog, fullUser))
            throw new NotBlogAuthorException("You do not have permission to delete this blog.");

        await repo.DeleteBlog(blog);
        // Delete blog file
        DeleteBlogFile(blog.Id);
        DeleteBanner(blog.Id);
        return blog;
    }

    public async Task<IEnumerable<BlogWithAuthor>> GetBlogsWithAuthor(string userId) {
        var user =  await userService.GetUserById(userId);
        if (user is null) throw new UserNotFoundException($"Cannot find user {userId}");
        
        return await repo.GetAllBlogsWithAuthor(user);
    }

    public async Task<IEnumerable<BlogWithContent>> SearchBlog(string searchText) {
        var blogs = await repo.FindBlogs(searchText);
        return await GetBlogsWithFile(blogs);
    }

    // Find user and blog, then add user to authors and send an update
    public async Task<BlogWithAuthor> AddAuthor(string blogId, string userId, UserDto loggedInUser) {
        var success = Guid.TryParse(blogId, out var guid);
        if (!success) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var blog = await repo.GetBlog(guid);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var loggedInUserFull = await userService.GetUserByUsername(loggedInUser.Username);
        if (loggedInUserFull is null) throw new UnauthorizedAccessException("Could not find logged in user");

        if (!UserIsAuthor(blog, loggedInUserFull)) {
            throw new NotBlogAuthorException("You do not have permission to add an author to this blog.");
        }

        var user = await userService.GetFullUser(userId);
        if (user is null) throw new UserNotFoundException($"Cannot find user with id: {userId}");

        if (UserIsAuthor(blog, user)) {
            return new BlogWithAuthor(blog);
        }

        blog.Authors.Add(user);
        await repo.UpdateBlog(blog);
        return new BlogWithAuthor(blog);
    }

    public async Task<BlogWithAuthor> AddMultipleAuthors(string blogId, IEnumerable<string> userIds,
        UserDto loggedInUser) {
        var success = Guid.TryParse(blogId, out var guid);
        if (!success) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var blog = await repo.GetBlog(guid);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var loggedInUserFull = await userService.GetUserByUsername(loggedInUser.Username);
        if (loggedInUserFull is null) throw new UnauthorizedAccessException("Could not find logged in user");

        if (!UserIsAuthor(blog, loggedInUserFull)) {
            throw new NotBlogAuthorException("You do not have permission to add an author to this blog.");
        }
        
        var list = new List<User>();
        foreach (var userId in userIds) {
            var user = await userService.GetFullUser(userId);
            if (user is null) throw new UserNotFoundException($"Cannot find user with id: {userId}");
            list.Add(user);
        }

        blog.Authors.AddRange(list);

        await repo.UpdateBlog(blog);
        return new BlogWithAuthor(blog);
    }
    
    public async Task<BlogWithAuthor> RemoveAuthor(string blogId, string userId, UserDto loggedInUser) {
        var success = Guid.TryParse(blogId, out var guid);
        if (!success) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var blog = await repo.GetBlog(guid);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var loggedInUserFull = await userService.GetUserByUsername(loggedInUser.Username);
        if (loggedInUserFull is null) throw new UnauthorizedAccessException("Could not find logged in user");

        if (!UserIsAuthor(blog, loggedInUserFull)) {
            throw new NotBlogAuthorException("You do not have permission to add an author to this blog.");
        }

        var user = await userService.GetFullUser(userId);
        if (user is null) throw new UserNotFoundException($"Cannot find user with id: {userId}");

        if (UserIsAuthor(blog, user)) {
            return new BlogWithAuthor(blog);
        }

        blog.Authors.Remove(user);
        await repo.UpdateBlog(blog);
        return new BlogWithAuthor(blog);
    }

    public async Task<BlogWithAuthor> RemoveMultipleAuthors(string blogId, IEnumerable<string> userIds,
        UserDto loggedInUser) {
        var success = Guid.TryParse(blogId, out var guid);
        if (!success) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var blog = await repo.GetBlog(guid);
        if (blog is null) throw new BlogNotFoundException($"Blog with ID {blogId} was not found.");

        var loggedInUserFull = await userService.GetUserByUsername(loggedInUser.Username);
        if (loggedInUserFull is null) throw new UnauthorizedAccessException("Could not find logged in user");

        if (!UserIsAuthor(blog, loggedInUserFull)) {
            throw new NotBlogAuthorException("You do not have permission to add an author to this blog.");
        }
        
        var list = new List<User>();
        foreach (var userId in userIds) {
            var user = await userService.GetFullUser(userId);
            if (user is null) throw new UserNotFoundException($"Cannot find user with id: {userId}");
            list.Add(user);
        }

        blog.Authors.RemoveAll(user => list.Contains(user));

        await repo.UpdateBlog(blog);
        return new BlogWithAuthor(blog);
    }

    /// <summary>
    ///     Returns a BlogWithFile object from a given Blog object
    /// </summary>
    /// <param name="blog"></param>
    /// <returns>A blog with file content</returns>
    private static async Task<BlogWithContent> GetBlogWithContent(Blog blog) {
        var content = await BlogFilesService.GetFileContent(blog.Id.ToString());
        var newBlog = new BlogWithContent(blog, content);
        return newBlog;
    }

    private static async Task SaveBlogFile(Guid id, string fileContent) =>
        await BlogFilesService.SaveFile(id.ToString(), fileContent);

    private async Task<IEnumerable<BlogWithContent>> GetBlogsWithFile(IEnumerable<Blog> blogs) {
        var blogsWithFile = new List<BlogWithContent>();

        foreach (var blog in blogs) {
            var blogContent = await GetBlogWithContent(blog);
            blogsWithFile.Add(blogContent);
        }

        return blogsWithFile;
    }

    private static void DeleteBlogFile(Guid blogId) => BlogFilesService.DeleteFile(blogId.ToString());

    private static bool UserIsAuthor(Blog blog, User user) => blog.Authors.Exists(author => author.Id == user.Id);

    private static async Task SaveBanner(Guid id, IFormFile fileContent) {
        var extension = Path.GetExtension(fileContent.FileName);
        await FileService.SaveFile(id.ToString(), fileContent, "blogs/banners", extension);
    }

    private static void DeleteBanner(Guid id) => FileService.DeleteFile(id.ToString(), "blogs/banners", ".*");

    private static async Task<string> GetBannerContent(Guid guid) {
        return await FileService.GetFileContent(guid.ToString(), "blogs/banners", ".*");
    }

    public async Task<byte[]> GetBanner(Guid guid) {
        return await FileService.GetFileBytes(guid.ToString(), "blogs/banners", ".*");
    }
}