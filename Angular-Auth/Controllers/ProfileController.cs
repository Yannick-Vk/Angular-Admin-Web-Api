using System.Security.Claims;
using Angular_Auth.Dto;
using Angular_Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/me")]
public class ProfileController : Controller {
    private readonly ILogger<ProfileController> _logger;
    private readonly IProfileService _profileService;

    public ProfileController(
        ILogger<ProfileController> logger,
        IProfileService profileService
    ) {
        _logger = logger;
        _profileService = profileService;
    }

    [HttpPut("change/email")]
    public async Task<IActionResult> UpdateEmail(UpdateEmailRequest request) {
        try {
            var userId = User.FindFirstValue("Id");
            if (userId == null) {
                return Unauthorized("Could not get userId from JWT Token");
            }

            await _profileService.UpdateEmail(userId, request.Email, request.Password);
            return Ok();
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("change/password")]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordRequest request) {
        try {
            var userId = User.FindFirstValue("Id");
            if (userId == null) {
                return Unauthorized("Could not get userId from JWT Token");
            }

            await _profileService.UpdatePassword(userId, request.NewPassword, request.Password);
            return Ok();
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("change/profile-picture")]
    public async Task<IActionResult> UploadProfilePicture([FromForm] ProfilePictureUpload upload) {
        try {
            var userId = User.FindFirstValue("Id");
            if (userId == null) {
                return Unauthorized("Could not get userId from JWT Token");
            }
            
            await _profileService.UploadProfilePicture(userId, upload);
            return Ok();
        }
        catch (Exception ex) {
            return Unauthorized(ex.Message);
        }
    }
    
    [HttpPost("profile-picture")]
    public async Task<IActionResult> GetProfilePicture() {
        try {
            var userId = User.FindFirstValue("Id");
            if (userId == null) {
                return Unauthorized("Could not get userId from JWT Token");
            }
            
            var image = await _profileService.GetProfilePicture(userId);
            if (image.Length == 0) return NotFound();

            return new FileContentResult(image, "image/jpeg");
        }
        catch (Exception ex) {
            return Unauthorized(ex.Message);
        }
    }
}