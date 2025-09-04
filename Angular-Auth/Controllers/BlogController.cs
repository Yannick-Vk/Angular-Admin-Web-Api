using Angular_Auth.Dto;
using Angular_Auth.Migrations;
using Angular_Auth.Models;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]s")]
public class BlogController(ILogger<BlogController> logger, IBlogService blogService) : Controller {
    [HttpPost]
    public async Task<IActionResult> UploadBlog(BlogUpload blogUpload) {
        var id = await blogService.UploadBlog(blogUpload);
        if (id is null) {
            return Problem("Failed to save blog ");
        }
        return Ok(id);
    }

    [HttpGet("{blogId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBlog(string blogId) {
        var blog = await blogService.GetBlog(blogId);
        if (blog is null) {
            return NotFound("Cannot find a blog with ID : " + blogId);
        }

        return Ok(blog);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<List<BlogWithFile>> GetAllBlogs() {
        return await blogService.GetAllBlogs();
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateBlog(BlogUpdate blog) {
        var result = await blogService.UpdateBlog(blog);
        if (result is null) {
            return NotFound("Cannot find a blog with ID : " + blog.Id);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBlog(string id) {
        var blog = await blogService.DeleteBlog(id);
        if (blog is null) {
            return NotFound($"Cannot find blog with ID: ${id}");
        }

        return Ok(blog);
    }

    [HttpGet("author/{username}")]
    public async Task<IEnumerable<Blog>> GetAllBlogsWithAuthor(string username) {
        return await blogService.GetBlogsWithAuthor(username);
    }
}