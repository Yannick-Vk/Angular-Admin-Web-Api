namespace Angular_Auth.Utils.tags;

public class Paragraph(IHtmlTag parent, IHtmlTag content) : HtmlTag("p", parent) {
    public Paragraph(IHtmlTag parent, Text text) : this(parent, text.ToTag()) { }

    public override string ToString() {
        return $"<p>{content}</p>";
    }
}