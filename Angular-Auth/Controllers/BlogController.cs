using Angular_Auth.Dto;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("api/v1/[controller]s")]
public class BlogController(IBlogService blogService) : Controller {

    [HttpPost]
    public async Task<IActionResult> UploadBlog(BlogUpload blog) {
        return Ok();
    }

    [HttpGet("{blogId}")]
    public async Task<IActionResult> GetBlog(string blogId) {
        return Ok();
    }
}