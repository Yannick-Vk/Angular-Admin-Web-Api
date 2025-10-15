using Angular_Auth.Dto.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using IMailService = Angular_Auth.Services.Interfaces.IMailService;

namespace Angular_Auth.Services;

public class MailService : IMailService {
    public async Task SendEmail(MimeMessage email) {
        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, false);

        // Note: only needed if the SMTP server requires authentication
        await smtp.AuthenticateAsync("shawn.froste.sf@gmail.com", "ufsw pzzt wnzn uabo");

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}