using Angular_Auth.Dto;
using Angular_Auth.Models;
using Angular_Auth.Repositories;

namespace Angular_Auth.Services;

public class BlogService(BlogRepository repo) : IBlogService {
    public Task<Blog> GetBlog() {
        throw new NotImplementedException();
    }

    public async Task UploadBlog(BlogUpload blogUpload) {
        var blog = new Blog {
            Id = Guid.NewGuid(),
            Title = blogUpload.Title,
            Description = blogUpload.Description
        };
        await repo.SaveBlog(blog);
        await SaveBlog(blog, blogUpload.File);
    }


    private async Task SaveBlog(Blog blog, string fileContent) {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        // Create the directory when it does not exist
        if (!Directory.Exists(uploadsFolder)) {
            Directory.CreateDirectory(uploadsFolder);
        }
        
        var uniqueFileName = blog.Id + ".md";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        var fileBytes = System.Text.Encoding.UTF8.GetBytes(fileContent);

        // Save the file.
        await using var stream = new FileStream(filePath, FileMode.Create);
        await stream.WriteAsync(fileBytes);
    }
}