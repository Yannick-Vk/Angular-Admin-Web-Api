namespace Angular_Auth.Utils.tags;

public class Paragraph(IHtmlTag content) : HtmlTag("p", content) {
    public Paragraph(Text text) : this(text.ToTag()) { }
}