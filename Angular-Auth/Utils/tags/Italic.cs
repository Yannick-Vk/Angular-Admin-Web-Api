namespace Angular_Auth.Utils.tags;

public class Italic(IHtmlComponent content) : HtmlTag("i", content) {
    public Italic(string text) : this(new Text(text)) { }
}