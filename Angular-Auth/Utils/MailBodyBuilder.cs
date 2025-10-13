using MimeKit;

namespace Angular_Auth.Utils;

public class MailBodyBuilder {
    private string _text = string.Empty;
    private readonly HtmlBuilder _htmlBuilder = new();
    
    public (string html, string text) Build() {
        return (_htmlBuilder.Build(), _text);
    }

    public MailBodyBuilder AddTitle(string text, ushort level) {
        _htmlBuilder.AddTitle(text, level);
        _text += text;
        return this;
    }
    
}