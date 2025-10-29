namespace Angular_Auth.Utils.tags;

public class Paragraph(IHtmlComponent content) : HtmlTag("p", content) {
    public Paragraph(Text text) : this(text.ToComponent()) { }
}