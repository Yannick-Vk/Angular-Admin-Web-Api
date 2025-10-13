using MimeKit;

namespace Angular_Auth.Services.Interfaces;

public interface IMailService {
    public Task SendEmail(MimeMessage email);
}