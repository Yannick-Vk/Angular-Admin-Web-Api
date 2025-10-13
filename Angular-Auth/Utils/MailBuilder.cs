using MimeKit;

namespace Angular_Auth.Utils;

// Alias the tuple as mailAdress
using _mailAddress = (string name, string email);
using _bodyContent = (string html, string text);

public class MailBuilder {
    private readonly MimeMessage _mail = new();

    public MimeMessage Build() {
        return _mail;
    }

    public MailBuilder To(_mailAddress to) {
        _mail.To.Add(new MailboxAddress(to.name, to.email));
        return this;
    }

    public MailBuilder From(_mailAddress from) {
        _mail.From.Add(new MailboxAddress(from.name, from.email));
        return this;
    }

    public MailBuilder Subject(string subject) {
        _mail.Subject = subject;
        return this;
    }

    private MailBuilder Content(_bodyContent body) {
        var bb = new BodyBuilder {
            TextBody = body.text,
            HtmlBody = body.html,
        };
        _mail.Body = bb.ToMessageBody();
        return this;
    }

    public MailBuilder Body(MailBodyBuilder bodyBuilder) {
        return Content(bodyBuilder.Build());
    }
}