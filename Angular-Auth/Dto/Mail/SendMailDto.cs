using MimeKit;

namespace Angular_Auth.Dto;

public class SendMailDto {
    public required string ToUsername { get; init; }
    public required string ToEmail { get; init; }
    public required string Subject { get; init; }
    public required BodyBuilder Body { get; init; } 
    
    public MimeMessage CreateEmail() {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Web Api", "js-blogger@yannick.be"));
        email.To.Add(new MailboxAddress(ToUsername, ToEmail));
        email.Subject = Subject;
        email.Body = Body.ToMessageBody();
        return email;
    }
    
    
}