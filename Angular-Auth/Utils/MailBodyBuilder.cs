using Angular_Auth.Utils.tags;
using MimeKit;

namespace Angular_Auth.Utils;

public class MailBodyBuilder {
    private string _text = string.Empty;
    private readonly HtmlBuilder _htmlBuilder = new();
    
    public (string html, string text) Build() {
        return (_htmlBuilder.Build(), _text);
    }

    public MailBodyBuilder AddTitle(Text text, ushort level) {
        _htmlBuilder.AddTitle(text, level);
        _text += text.text;
        return this;
    }
    
}