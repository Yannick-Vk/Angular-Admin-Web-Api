namespace Angular_Auth.Utils.tags;

public class Bold(IHtmlTag parent, IHtmlTag content) : HtmlTag("b", parent) {
    public Bold(IHtmlTag parent, string text) : this(parent, new Text(text)) { }

    public override string ToString() {
        return $"<b>{content}</b>";
    }
}