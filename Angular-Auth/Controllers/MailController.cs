using Angular_Auth.Dto.Mail;
using Angular_Auth.Services.Interfaces;
using Angular_Auth.Utils;
using Angular_Auth.Utils.tags;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using IMailService = Angular_Auth.Services.Interfaces.IMailService;

namespace Angular_Auth.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class MailController(
    ILogger<MailController> _logger,
    IAuthenticationService authService,
    IMailService mailService,
    ILoggerFactory loggerFactory) : ControllerBase {
    private readonly ILogger<MailBuilder> _mailBuilderLogger = loggerFactory.CreateLogger<MailBuilder>();

    [HttpPost("demo")]
    // Demo route for development, this route is to send a confirm account email
    public async Task<IActionResult> DemoMail() {
        try {
            var user = authService.GetUserFromClaimsPrincipal(HttpContext.User);
            if (user == null) {
                return Problem("Cannot find user token, user is not logged in.");
            }

            var mail = new MailBuilder(_mailBuilderLogger)
                .To((user.Username, user.Email))
                .From(("JS-Blogger api", "js-blogger@yannick.be"))
                .Subject(("Demo mail with builders"))
                /*.Body(new MailBodyBuilder()
                    .AddTitle(new Text("Welcome to Js-Blogger"), 1)
                    .AddParagraph(new Text("Hi there! We welcome you to our blog!"))
                    .AddLink("Link to our site", "site")
                    .AddDiv(div => div
                        .AddTitle(new Text("Welcome to the newsletter section"), 2)
                        .AddParagraph(new Text("Hi from a div!")))
                )
                */
                //.AddFiles("./Mails/demo/demo.html", "./Mails/demo/demo.jeff")
                .AddFiles("demo")
                .Build();
            
            //_logger.LogInformation("Mail: {mail}", mail);

            await mailService.SendEmail(mail);
            return Ok(mail.ToString());
        }
        catch (Exception ex) {
            return StatusCode(500, ex);
        }
    }
}