using System.Security.Claims;
using Angular_Auth.Dto;
using Angular_Auth.Dto.Users;
using Angular_Auth.Services;
using Angular_Auth.Services.Interfaces;
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

            await _profileService.UpdateEmail(userId, request.Email);
            return Ok();
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("change/username")]
    public async Task<IActionResult> UpdateUsername(UpdateUsernameRequest request) {
        try {
            var userId = User.FindFirstValue("Id");
            if (userId == null) {
                return Unauthorized("Could not get userId from JWT Token");
            }

            var success = await _profileService.UpdateUsername(userId, request.Username);
            if (success) return Ok();
            return BadRequest("Username already exists");
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

            await _profileService.UpdatePassword(userId, request.Password, request.NewPassword);
            return Ok();
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("change/profile-picture")]
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

    [HttpGet("profile-picture")]
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

    [HttpDelete("profile-picture")]
    public async Task<IActionResult> DeleteProfilePicture() {
        try {
            var userId = User.FindFirstValue("Id");
            if (userId == null) {
                return Unauthorized("Could not get userId from JWT Token");
            }

            await _profileService.DeleteProfilePicture(userId);
            return Ok();
        }
        catch (Exception ex) {
            return Unauthorized(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("password/reset/{email}")]
    public async Task<IActionResult> ResetPassword(string email) {
        try {
            await _profileService.SendResetPasswordMail(email);

            return Ok();
        }
        catch (Exception ex) {
            return BadRequest("Failed to reset password. " + ex.Message);
        }
    }

    [HttpPost("password/confirm/")]
    public async Task<IActionResult> ConfirmPassword(string userId, string token, string newPassword) {
        try {
            var result = await _profileService.ConfirmResetPassword(userId, token, newPassword);

            if (result.Succeeded) {
                return Ok();
            }

            return BadRequest(result.Errors);
        }
        catch (Exception ex) {
            return BadRequest("Failed to confirm password reset: " + ex.Message);
        }
    }
}