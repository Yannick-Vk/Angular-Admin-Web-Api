namespace Angular_Auth.Utils.tags;

public class Bold(IHtmlTag parent, IHtmlTag content) : HtmlTag("b", parent) {
    private IHtmlTag _content { get; } = content;

    public Bold(IHtmlTag parent, string text) : this(parent, new Text(text)) { }

    public override string ToString() {
        return $"<b>{_content}</b>";
    }
}