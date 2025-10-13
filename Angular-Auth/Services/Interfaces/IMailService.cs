using Angular_Auth.Dto.Mail;
using MimeKit;

namespace Angular_Auth.Services.Interfaces;

public interface IMailService {
    public Task SendEmail(MimeMessage email);
    public Task SendEmail(SendMailDto dto) => SendEmail(CreateEmail(dto));

    public static MimeMessage CreateEmail(SendMailDto dto) {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Web Api", "js-blogger@yannick.be"));
        email.To.Add(new MailboxAddress(dto.ToUsername, dto.ToEmail));
        email.Subject = dto.Subject;
        email.Body = dto.Body.ToMessageBody();
        return email;
    }
}