using Angular_Auth.Dto;
using Angular_Auth.Models;

namespace Angular_Auth.Services;

public interface IBlogService {
    Task<IEnumerable<BlogWithContent>> GetAllBlogs();
    Task<BlogWithContent?> GetBlog(string id);
    Task<Guid> UploadBlog(UserDto user, BlogUpload blogUpload);
    Task<BlogWithContent> UpdateBlog(BlogUpdate dto, UserDto loggedInUser);
    Task<Blog> DeleteBlog(string id, UserDto user);
    Task<IEnumerable<BlogWithAuthor>> GetBlogsWithAuthor(string username);
    Task<IEnumerable<BlogWithContent>> SearchBlog(string searchText);
    Task<BlogWithAuthor> AddAuthor(string blogId, string userId, UserDto loggedInUser);
    Task<byte[]> GetBanner(Guid guid);
}