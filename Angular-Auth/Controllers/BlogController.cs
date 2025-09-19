using Angular_Auth.Dto;
using Angular_Auth.Exceptions;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Angular_Auth.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]s")]
public class BlogController(
    ILogger<BlogController> logger,
    IBlogService blogService,
    IAuthenticationService authService) : Controller {
    [HttpPost]
    public async Task<IActionResult> UploadBlog(BlogUpload blogUpload) {
        var id = await blogService.UploadBlog(blogUpload);
        if (id is null) return Problem("Failed to save blog ");

        return Ok(id);
    }

    [HttpGet("{blogId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBlog(string blogId) {
        var blog = await blogService.GetBlog(blogId);
        if (blog is null) return NotFound("Cannot find a blog with ID : " + blogId);

        return Ok(blog);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IEnumerable<BlogWithFile>> GetAllBlogs() {
        return await blogService.GetAllBlogs();
    }
    
    [HttpPatch]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> UpdateBlog(BlogUpdate blog) {
        var user = authService.GetUserFromRequest(Request);
        if (user is null) {
            return Unauthorized();
        }

        try {
            var result = await blogService.UpdateBlog(blog, user);
            return Ok(result);
        }
        catch (BlogNotFoundException e) {
            return NotFound(e.Message);
        }
        catch (NotBlogAuthorException e) {
            logger.LogError("{message}", e.Message);
            return Forbid();
        }
        catch (Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(Status200OK)]
    [ProducesResponseType(Status401Unauthorized)]
    [ProducesResponseType(Status403Forbidden)]
    [ProducesResponseType(Status404NotFound)]
    public async Task<IActionResult> DeleteBlog(string id) {
        var user  = authService.GetUserFromRequest(Request);
        if (user is null) {
            return Unauthorized();
        }

        try {
            var blog = await blogService.DeleteBlog(id, user);
            if (blog is null) return NotFound($"Cannot find blog with ID: ${id}");

            return Ok(blog);
        }
        catch (BlogNotFoundException e) {
            return NotFound(e.Message);
        }
        catch (NotBlogAuthorException e) {
            logger.LogError("{message}", e.Message);
            return Forbid();
        }
        catch (Exception e) {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("author/me")]
    public async Task<IActionResult> GetAllBlogsWithAuthor() {
        var user = authService.GetUserFromRequest(Request);
        if (user is null) {
            return Unauthorized();
        }
        
        return Ok(await blogService.GetBlogsWithAuthor(user.Username!));
    }

    [AllowAnonymous]
    [HttpGet("search/{searchText}")]
    public async Task<IActionResult> GetAllBlogsWithSearch(string searchText) {
        var blogs = await blogService.SearchBlog(searchText);
        if (!blogs.Any()) return NotFound("Cannot find a blog with search text : " + searchText);

        return Ok(blogs);
    }
}