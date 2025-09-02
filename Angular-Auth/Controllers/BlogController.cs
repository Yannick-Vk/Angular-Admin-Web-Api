using Angular_Auth.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[ApiController]
[Route("api/v1/[controller]s")]
public class BlogController : Controller {

    [HttpPost]
    public async Task<IActionResult> UploadBlog(BlogUpload blog) {
        return Ok();
    }

    [HttpGet("{blogId}")]
    public async Task<IActionResult> GetBlog(string blogId) {
        return Ok();
    }
}