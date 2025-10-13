using Angular_Auth.Dto;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace Angular_Auth.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/v1/[controller]")]
public class MailController : ControllerBase {
    [HttpGet("demo")]
    public async Task<IActionResult> DemoMail(string username, string email) {
        return await SendEmail(new SendMailDto {
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
    }

    private async Task<IActionResult> SendEmail(MimeMessage email) {
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, false);

        // Note: only needed if the SMTP server requires authentication
        await smtp.AuthenticateAsync("8dc84259c8b865", "4556e06746fa6b");

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
        return Ok($"Mail '{email.Subject}' has been sent to {string.Join(",", email.To)}");
    }
}