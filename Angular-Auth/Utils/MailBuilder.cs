using MimeKit;

namespace Angular_Auth.Utils;

// Alias the tuple as mailAdress
using _mailAddress = (string name, string email);
using _bodyContent = (string html, string text);

public class MailBuilder(ILogger<MailBuilder> _logger) {
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

    /// <summary>
    /// Adds both a html file and a text file to mail body, if either file is not found, nothing get added
    /// </summary>
    /// <param name="htmlFile">The path to the html file</param>
    /// <param name="textFile">The path to the text file</param>
    /// <returns>Mailbuilder chaining</returns>
    public MailBuilder AddFiles(string htmlFile, string textFile) {
        if (!File.Exists(htmlFile)) {
            _logger.LogWarning("Html File [{HtmlFile}] was not found", htmlFile);
            return this;
        }

        if (!File.Exists(textFile)) {
            _logger.LogWarning("Text File [{TextFile}] was not found", textFile);
            return this;
        }

        Content((File.ReadAllText(htmlFile), File.ReadAllText(textFile)));
        return this;
    }

    public MailBuilder AddFiles(string mailName) {

        var basePath = "./Mails/" + mailName + "/";
        //_logger.LogInformation("{path}",  basePath);
        
        return AddFiles(basePath + mailName + ".html", basePath + mailName + ".txt");
    }
}