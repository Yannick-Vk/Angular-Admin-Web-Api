using MimeKit;
using IMailService = Angular_Auth.Services.Interfaces.IMailService;

namespace Angular_Auth.Dto.Mail;

public class SendMailDto {
    public required string ToUsername { get; init; }
    public required string ToEmail { get; init; }
    public required string Subject { get; init; }
    public required BodyBuilder Body { get; init; }

    public MimeMessage CreateEmail() {
        return IMailService.CreateEmail(this);
    }
}