using Angular_Auth.Dto;
using Angular_Auth.Models;

namespace Angular_Auth.Services;

public interface IBlogService {
    public Task<Blog> GetBlog();
    public Task UploadBlog(BlogUpload blogUpload);
}