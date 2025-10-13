using System.Collections;
using Angular_Auth.Utils.tags;
using MimeKit;

namespace Angular_Auth.Utils;

public class MailBodyBuilder : IEnumerable {
    private string _text = string.Empty;
    private readonly HtmlBuilder _htmlBuilder = new();

    public (string html, string text) Build() {
        return (_htmlBuilder.Build(), _text);
    }

    public MailBodyBuilder AddTitle(Text text, ushort level) {
        _add_text(text);
        _htmlBuilder.AddTitle(text, level);
        return this;
    }

    public MailBodyBuilder AddParagraph(Text text) {
        _add_text(text);
        _htmlBuilder.AddParagraph(text);
        return this;
    }

    private void _add_text(string text) {
        _text += text + " ";
    }

    private void _add_text(Text text) => _add_text(text.text);

    public MailBodyBuilder AddLink(Text text, string link) {
        _add_text(text + " '" + link + "'");
        _htmlBuilder.AddLink(text, link);
        return this;
    }

    public MailBodyBuilder AddLink(string text, string link) => AddLink(new Text(text), link);

    public MailBodyBuilder AddDiv(IHtmlTag content) {
        _htmlBuilder.AddDiv(content);
        return this;
    }

    public MailBodyBuilder AddDiv(MailBodyBuilder bodyBuilder) {
        _htmlBuilder.AddDiv(bodyBuilder);
        return this;
    }

    public IEnumerator GetEnumerator() {
        return _htmlBuilder.Tree.Children.GetEnumerator();
    }
}