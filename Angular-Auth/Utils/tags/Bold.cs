namespace Angular_Auth.Utils.tags;

public class Bold(IHtmlTag parent, IHtmlTag content) : HtmlTag("b", parent, content) {
    public Bold(IHtmlTag parent, string text) : this(parent, new Text(text)) { }
}