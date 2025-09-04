using Angular_Auth.Dto;
using Angular_Auth.Models;
using Angular_Auth.Repositories;

namespace Angular_Auth.Services;

public class BlogService(BlogRepository repo, IUserService userService) : IBlogService {
    public async Task<List<BlogWithFile>> GetAllBlogs() {
        var blogs = await repo.GetAllBlogs();
        var blogsWithFile = new List<BlogWithFile>();

        foreach (var blog in blogs) {
            var blogContent = await GetBlogWithContent(blog);
            blogsWithFile.Add(blogContent);
        }

        return blogsWithFile;
    }

    /// <summary>
    /// Returns a BlogWithFile object from a given Blog object
    /// </summary>
    /// <param name="blog"></param>
    /// <returns>A blog with file content</returns>
    private async Task<BlogWithFile> GetBlogWithContent(Blog blog) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        var uniqueFileName = blog.Id + ".md";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Initialize an empty string, if the file exist add the contents
        var content = string.Empty;
        if (File.Exists(filePath)) {
            content = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8);
        }

        var newBlog = new BlogWithFile() {
            Id = blog.Id,
            Title = blog.Title,
            Description = blog.Description,
            BlogContent = content,
            Author = blog.Author.UserName ?? "NULL USER",
            CreatedAt = blog.CreatedAt,
            UpdatedAt = blog.UpdatedAt,
        };
        return newBlog;
    }

    public async Task<BlogWithFile?> GetBlog(string id) {
        var blog = await repo.GetBlog(id);
        if (blog is null) {
            return null;
        }

        return await GetBlogWithContent(blog);
    }

    public async Task<Guid?> UploadBlog(BlogUpload blogUpload) {
        var author = await userService.GetFullUser(blogUpload.Author);
        if (author is null) return null;

        var blog = new Blog {
            Id = Guid.NewGuid(),
            Title = blogUpload.Title,
            Description = blogUpload.Description,
            CreatedAt = DateTime.Now,
            Author = author,
        };
        await repo.SaveBlog(blog);
        await SaveBlog(blog.Id, blogUpload.File);
        return blog.Id;
    }


    private async Task SaveBlog(Guid id, string fileContent) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        // Create the directory when it does not exist
        if (!Directory.Exists(uploadsFolder)) {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = id + ".md";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        var fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);

        // Save the file.
        await using var stream = new FileStream(filePath, FileMode.Create);
        await stream.WriteAsync(fileBytes);
    }

    public async Task<BlogWithFile?> UpdateBlog(BlogUpdate updateBlog) {
        var result = await repo.UpdateBlog(updateBlog);
        if (result is null) return null;

        if (updateBlog.UpdatedFileContent is not null) {
            await SaveBlog(result.Id, updateBlog.UpdatedFileContent);
        }

        return await GetBlogWithContent(result);
    }

    public async Task<Blog?> DeleteBlog(string id) {
        return await repo.DeleteBlog(id);
    }

    public async Task<IEnumerable<Blog>> GetBlogsWithAuthor(string username) {
        return await repo.GetAllBlogsWithAuthor(username);
    }
}