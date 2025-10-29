using MailKit.Net.Smtp;
using MimeKit;
using IMailService = Angular_Auth.Services.Interfaces.IMailService;

namespace Angular_Auth.Services;

public class MailService : IMailService {
    private readonly IConfiguration _configuration;

    public MailService(IConfiguration configuration) {
        _configuration = configuration;
    }

    public async Task SendEmail(MimeMessage email) {
        using var smtp = new SmtpClient();
        var success = int.TryParse(_configuration["MailSettings:Port"], out var port);
        if (!success) {
            throw new Exception($"Mailserver error, failed to parse port['{port}'] to a number");
        }

        await smtp.ConnectAsync(_configuration["MailSettings:Host"], port, false);

        // Note: only needed if the SMTP server requires authentication
        await smtp.AuthenticateAsync(_configuration["MailSettings:Username"], _configuration["MailSettings:Password"]);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}