using Angular_Auth.Dto.Mail;
using Angular_Auth.Utils;
using MimeKit;

namespace Angular_Auth.Services.Interfaces;

public interface IMailService {
    private const string FromName = "JS Blogger"; 
    private const string FromAddress = "js-blogger@yannick.be";
    public Task SendEmail(MimeMessage email);
    public Task SendEmail(SendMailDto dto) => SendEmail(CreateEmail(dto));

    public static MimeMessage CreateEmail(SendMailDto dto) => CreateEmail(dto, (FromName, FromAddress));

    public static MimeMessage CreateEmail(SendMailDto dto, (string name, string address) from) {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(from.name, from.address));
        email.To.Add(new MailboxAddress(dto.ToUsername, dto.ToEmail));
        email.Subject = dto.Subject;
        email.Body = dto.Body.ToMessageBody();
        return email;
    }

    public static (string name, string address) From() => (FromName, FromAddress);
}