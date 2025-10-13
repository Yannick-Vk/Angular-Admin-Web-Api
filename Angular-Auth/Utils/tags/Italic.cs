namespace Angular_Auth.Utils.tags;

public class Italic(IHtmlTag parent, IHtmlTag content) : HtmlTag("i", parent, content) {
    public Italic(IHtmlTag parent, string text) : this(parent, new Text(text)) { }
}