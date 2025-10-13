using Angular_Auth.Dto.Mail;
using Angular_Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using IMailService = Angular_Auth.Services.Interfaces.IMailService;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class MailController(IAuthenticationService authService, IMailService mailService) : ControllerBase {
    [HttpPost("demo")]
    // Demo route for development, this route is to send a confirm account email
    public async Task<IActionResult> DemoMail() {
        try {
            var user = authService.GetUserFromClaimsPrincipal(HttpContext.User);
            if (user == null) {
                return Problem("Cannot find user token, user is not logged in.");
            }

            const string linkAddr = "#";

            await mailService.SendEmail(new SendMailDto {
                ToUsername = user.Username,
                ToEmail = user.Email,
                Subject = "Confirm your email",
                Body = new BodyBuilder {
                    HtmlBody =
                        $"""
                         <html>
                         <h1>Welcome to JS-Blogger!</h1>
                         <p>Please confirm your email by clicking the following link <a href=\"{linkAddr}\">Confirm my email</a></p>
                         </br>
                         <p><b>Thanks</b></p>
                         <p><i>Dev Team</i></p></html>"
                         """,
                    TextBody = $"""
                                Hey, welcome to JS-Blogger!
                                Please confirm your email with this link: {linkAddr}

                                Thanks
                                -- Dev Team
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