using Angular_Auth.Dto;
using Angular_Auth.Models;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("api/v1/[controller]s")]
public class BlogController(ILogger<BlogController> logger, IBlogService blogService) : Controller {

    [HttpPost]
    public async Task<IActionResult> UploadBlog(BlogUpload blog) {
        await blogService.UploadBlog(blog);
        return Ok();
    }

    [HttpGet("{blogId}")]
    public async Task<IActionResult> GetBlog(string blogId) {
        var blog = await blogService.GetBlog(blogId);
        if (blog is null) {
            return NotFound("Cannot find a blog with ID : " + blogId);
        }
        return Ok(blog);
    }

    [HttpGet]
    public async Task<List<BlogWithFile>> GetAllBlogs() {
        return await blogService.GetAllBlogs();
    }
}