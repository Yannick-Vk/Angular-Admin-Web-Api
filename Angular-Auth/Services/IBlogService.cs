using Angular_Auth.Dto;
using Angular_Auth.Models;

namespace Angular_Auth.Services;

public interface IBlogService {
    public Task<BlogWithFile?> GetBlog(string id);
    public Task<Guid?> UploadBlog(BlogUpload blogUpload);
    public Task<IEnumerable<BlogWithFile>> GetAllBlogs();
    public Task<BlogWithFile> UpdateBlog(BlogUpdate dto, UserDto loggedInUser);
    public Task<Blog?> DeleteBlog(string id, UserDto user);
    public Task<IEnumerable<BlogWithAuthor>> GetBlogsWithAuthor(string username);
    public Task<IEnumerable<BlogWithFile>> SearchBlog(string searchText);
    public Task AddAuthor(string blogId, string userId);
}