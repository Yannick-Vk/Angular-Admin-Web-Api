namespace Angular_Auth.Utils.tags;

public class Bold(IHtmlTag content) : HtmlTag("b", content) {
    public Bold(string text) : this(new Text(text)) { }
}