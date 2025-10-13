namespace Angular_Auth.Utils.tags;

public class Link(IHtmlTag parent, IHtmlTag content, string link) : HtmlTag("a", parent, content) {
    public Link(IHtmlTag parent, string text, string link) : this(parent, new Text(text), link) { }

    public override string ToString() {
        return $"<a href=\"{link}\">{Content}</a>";
    }
}