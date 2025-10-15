using MimeKit;

namespace Angular_Auth.Utils;

// Type aliases 
using _mailAddress = (string name, string email);
using _bodyContent = (string html, string text);
using _mappings = List<(string mapping, string value)>;

/// <summary>
/// Builder pattern for creating mails
/// </summary>
/// <param name="_logger">The logger</param>
public class MailBuilder(ILogger<MailBuilder> _logger) {
    private readonly MimeMessage _mail = new();

    /// <summary>
    /// Builds the mail
    /// </summary>
    /// <returns>MimeMessage, Mail object</returns>
    public MimeMessage Build() {
        return _mail;
    }

    /// <summary>
    /// Set the 'To' email address with name and email
    /// </summary>
    /// <param name="to">The mail address to send the mail to</param>
    /// <returns>MailBuilder chain</returns>
    public MailBuilder To(_mailAddress to) {
        _mail.To.Add(new MailboxAddress(to.name, to.email));
        return this;
    }

    /// <summary>
    /// Set the 'From' email address with name and email
    /// </summary>
    /// <param name="from">The mail address where the mail was sent from</param>
    /// <returns>MailBuilder chain</returns>
    public MailBuilder From(_mailAddress from) {
        _mail.From.Add(new MailboxAddress(from.name, from.email));
        return this;
    }

    /// <summary>
    /// Set the email subject
    /// </summary>
    /// <param name="subject">The subject of the mail</param>
    /// <returns>MailBuilder chain</returns>
    public MailBuilder Subject(string subject) {
        _mail.Subject = subject;
        return this;
    }

    /// <summary>
    /// Sets the mail html and text with a BodyBuilder
    /// </summary>
    /// <param name="body">A tuple of html as a string and plain text as a string</param>
    /// <returns>MailBuilder chain</returns>
    private MailBuilder Content(_bodyContent body) {
        var bb = new BodyBuilder {
            TextBody = body.text,
            HtmlBody = body.html,
        };
        _mail.Body = bb.ToMessageBody();
        return this;
    }

    /// <summary>
    /// Creates a html body with a builder pattern, adds text automatically in the background
    /// </summary>
    /// <param name="bodyBuilder">A bodybuilder class to build</param>
    /// <returns>MailBuilder chain</returns>
    public MailBuilder Body(MailBodyBuilder bodyBuilder) {
        return Content(bodyBuilder.Build());
    }

    /// <summary>
    /// Adds both a html file and a text file to mail body, if either file is not found, nothing get added
    /// </summary>
    /// <param name="htmlFile">The path to the html file</param>
    /// <param name="textFile">The path to the text file</param>
    /// <returns>MailBuilder chaining</returns>
    public MailBuilder AddFiles(string htmlFile, string textFile) {
        try {
            return AddFilesOrThrow(htmlFile, textFile);
        }
        catch (Exception e) {
            _logger.LogError(e, "Failed to add files");
        }

        return this;
    }

    /// <summary>
    /// Packed overloaded function, adds both html and text file to the mail body
    /// </summary>
    /// <param name="files">A tuple of a htmlFile and textFile</param>
    /// <returns>MailBuilder chain</returns>
    public MailBuilder AddFiles((string htmlFile, string textFile) files) {
        return AddFiles(files.htmlFile, files.textFile);
    }

    public MailBuilder AddFilesOrThrow(string htmlFile, string textFile) {
        if (!File.Exists(htmlFile)) {
            throw new HtmlFileNotFoundException(htmlFile);
        }

        if (!File.Exists(textFile)) {
            throw new TextFileNotFoundException(textFile);
        }

        Content((File.ReadAllText(htmlFile), File.ReadAllText(textFile)));
        return this;
    }

    /// <summary>
    /// Add both html and text files to the mail body, this is done via mail name as it will search for the folder
    /// in the mails directory with the same name and then the files ending in .html, .txt
    /// </summary>
    /// <param name="mailName">The name of the mail folder and files</param>
    /// <returns>MailBuilder chain</returns>
    public MailBuilder AddFiles(string mailName) {
        try {
            return AddFilesOrThrow(mailName);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Failed to add files.");
        }

        return this;
    }

    /// <summary>
    /// Tries to add files via name to the mail or throws if it cannot find the files
    /// </summary>
    /// <param name="mailName">The name of the folder and files to add</param>
    /// <returns>MailBuilder chain</returns>
    public MailBuilder AddFilesOrThrow(string mailName) {
        var basePath = "./Mails/" + mailName + "/";
        return AddFilesOrThrow(basePath + mailName + ".html", basePath + mailName + ".txt");
    }

    /// <summary>
    /// Packed overloaded function, adds both html and text file to the mail body
    /// </summary>
    /// <param name="files">A tuple of a htmlFile and textFile</param>
    /// <returns>MailBuilder chain</returns>
    public MailBuilder AddFilesOrThrow((string htmlFile, string textFile) files) {
        return AddFilesOrThrow(files.htmlFile, files.textFile);
    }

    private string FormatHtml(string html, _mappings mappings) {
        return mappings.Aggregate(html, (current, mapping) => current.Replace(mapping.mapping, mapping.value));
    }

    /// <summary>
    /// An exception for when you can't find a file
    /// </summary>
    /// <param name="filename">The file that can't be found</param>
    /// <param name="fileType">The file type, (html, text ...)</param>
    private class FileNotFoundException(string filename, string fileType) : Exception(
        $"{fileType} File [{filename}] was not found"
    );

    /// <summary>
    /// An exception when a Html file cannot be found
    /// </summary>
    /// <param name="filename">The name of the file that cannot be found</param>
    private class HtmlFileNotFoundException(string filename) : FileNotFoundException(filename, "Html");

    /// <summary>
    /// An exception when a Text file cannot be found
    /// </summary>
    /// <param name="filename">The name of the file that cannot be found</param>
    private class TextFileNotFoundException(string filename) : FileNotFoundException(filename, "Text");
}