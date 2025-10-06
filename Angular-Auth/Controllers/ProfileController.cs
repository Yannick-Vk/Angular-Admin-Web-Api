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
                return Unauthorized("Invalid user id");
            }

            await _profileService.UpdateEmail(userId, request.Email);
            return Ok();
        }
        catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}