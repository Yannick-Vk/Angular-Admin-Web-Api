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
        logger.LogInformation("Trying to upload blog");
        await blogService.UploadBlog(blog);
        return Ok();
    }

    [HttpGet("{blogId}")]
    public async Task<IActionResult> GetBlog(string blogId) {
        return Ok();
    }

    [HttpGet]
    public async Task<List<BlogWithFile>> GetAllBlogs() {
        return await blogService.GetAllBlogs();
    }
}