namespace Angular_Auth.Utils.tags;

public class Bold(IHtmlComponent content) : HtmlTag("b", content) {
    public Bold(string text) : this(new Text(text)) { }
}