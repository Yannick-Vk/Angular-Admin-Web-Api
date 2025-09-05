using Angular_Auth.Dto;
using Angular_Auth.Models;

namespace Angular_Auth.Services;

public interface IBlogService {
    public Task<BlogWithFile?> GetBlog(string id);
    public Task<Guid?> UploadBlog(BlogUpload blogUpload);
    public Task<IEnumerable<BlogWithFile>> GetAllBlogs();
    public Task<BlogWithFile?> UpdateBlog(BlogUpdate blog);
    public Task<Blog?> DeleteBlog(string id);
    public Task<IEnumerable<Blog>> GetBlogsWithAuthor(string username);
    public Task<IEnumerable<BlogWithFile>> SearchBlog(string searchText);
}