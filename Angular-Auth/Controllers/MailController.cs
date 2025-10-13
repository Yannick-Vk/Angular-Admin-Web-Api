using Angular_Auth.Dto.Mail;
using Angular_Auth.Services.Interfaces;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using IMailService = Angular_Auth.Services.Interfaces.IMailService;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class MailController(IAuthenticationService authService, IMailService mailService) : ControllerBase {
    [HttpGet("demo")]
    public async Task<IActionResult> DemoMail(string username, string email) {
        try {
            var user = authService.GetUserFromClaimsPrincipal(HttpContext.User);
            if (user == null) {
                return Unauthorized("Cannot find user, user is not logged in.");
            }
            
            await mailService.SendEmail(new SendMailDto {
                ToUsername = username,
                ToEmail = email,
                Subject = "Demo Mail",
                Body = new BodyBuilder() {
                    HtmlBody = "<p>Hey,<br>Just wanted to say hi all the way from the land of C#.<br>-- Code guy</p>",
                    TextBody = """
                               Hey, Just wanted to say hi all the way from the land of C#.
                               -- Code guy
                               """,
                }
            }.CreateEmail());
            return Ok();
        }
        catch (Exception ex) {
            return StatusCode(500, ex);
        }
    }
}