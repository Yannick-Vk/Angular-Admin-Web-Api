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
        var htmlBody = "<p>Hey,<br>Just wanted to say hi all the way from the land of C#.<br>-- Code guy</p>";
        var textBody = """
                       Hey, Just wanted to say hi all the way from the land of C#.
                       -- Code guy
                       """;
        return await SendEmail(username, email,  "Demo mail from our web api", htmlBody, textBody);
    }

    private async Task<IActionResult> SendEmail(string username, string emailAddress, string subject, string htmlBody, string textBody) {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Web Api", "js-blogger@yannick.be"));
        email.To.Add(new MailboxAddress(username, emailAddress));
        email.Subject = subject; 
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("sandbox.smtp.mailtrap.io", 2525, false);

        // Note: only needed if the SMTP server requires authentication
        await smtp.AuthenticateAsync("8dc84259c8b865", "4556e06746fa6b");
        var bodyBuilder = new BodyBuilder {
            HtmlBody = htmlBody,
            TextBody = textBody, 
        };

        email.Body = bodyBuilder.ToMessageBody();

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
        return Ok($"Mail '{email.Subject}' has been sent to {String.Join(",", email.To)}");
    }
}