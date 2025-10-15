using System.Collections;
using Angular_Auth.Utils.tags;
using MimeKit;

namespace Angular_Auth.Utils;

public class MailBodyBuilder : IEnumerable {
    private readonly List<string> _text = [];
    private readonly HtmlBuilder _htmlBuilder = new();

    public (string html, string text) Build() {
        return (_htmlBuilder.Build(), GetText());
    }

    private string GetText() => GetText(_text);

    private static string GetText(List<string> text) {
        return string.Join("\n", text);
    }

    public MailBodyBuilder AddTitle(Text text, ushort level) {
        if (level != 1) 
            _add_text("");
        
        _add_text(text);
        _htmlBuilder.AddTitle(text, level);
        return this;
    }

    public MailBodyBuilder AddParagraph(Text text) {
        _add_text(text);
        _htmlBuilder.AddParagraph(text);
        return this;
    }

    private void _add_text(string text) => _text.Add(text);
    private void _add_text(Text text) => _text.Add(text.text);

    public MailBodyBuilder AddLink(Text text, string link) {
        _add_text(text + " '" + link + "'");
        _htmlBuilder.AddLink(text, link);
        return this;
    }

    public MailBodyBuilder AddLink(string text, string link) => AddLink(new Text(text), link);

    public MailBodyBuilder AddDiv(Action<MailBodyBuilder> builderAction) {
        var divBuilder = new MailBodyBuilder();
        builderAction(divBuilder);

        _add_text(GetText(divBuilder._text));

        var div = new Div();
        foreach (var child in divBuilder._htmlBuilder.Tree.Children) {
            div.Add(child);
        }
        _htmlBuilder.Tree.Add(div);

        return this;
    }

    public IEnumerator GetEnumerator() {
        return _htmlBuilder.Tree.Children.GetEnumerator();
    }
}