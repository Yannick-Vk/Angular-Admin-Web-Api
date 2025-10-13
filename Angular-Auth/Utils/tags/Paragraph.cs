namespace Angular_Auth.Utils.tags;

public class Paragraph(IHtmlTag parent, IHtmlTag content) : HtmlTag("p", parent, content) {
    public Paragraph(IHtmlTag parent, Text text) : this(parent, text.ToTag()) { }
}