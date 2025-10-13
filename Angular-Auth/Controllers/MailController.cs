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

    [HttpGet]
    public async Task<IActionResult> SendMail() {
        var email = new MimeMessage {
            Subject = "Hello from the Web Api",
            Body = new TextPart() {
                Text = "Hello from the Web Api. Welcome to our mail service!"
            },
        };
        email.From.Add(new MailboxAddress("Web Api", "js-blogger@yannick.be"));
        email.To.Add(new MailboxAddress("Mr to", "to@example.com"));
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, false);
        
        // Note: only needed if the SMTP server requires authentication
        await smtp.AuthenticateAsync("8dc84259c8b865", "4556e06746fa6b");
        
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
        return Ok("Mail has been sent");
    }
}