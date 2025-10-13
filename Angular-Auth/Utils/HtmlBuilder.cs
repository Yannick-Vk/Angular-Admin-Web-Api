using Angular_Auth.Utils.tags;

namespace Angular_Auth.Utils;

public class HtmlBuilder {
    public Root Tree { get; private set; }= new();

    public string Build() {
        return Tree.ToString();
    }

    public void AddTitle(Text text, ushort level) {
        Tree.Add(new TitleTag(Tree, text, level));
    }

    public void AddParagraph(Text text) {
        Tree.Add(new Paragraph(Tree, text));
    }

    public void AddLink(Text text, string link) {
        Tree.Add(new Link(Tree, text, link));
    }

    public void AddDiv(IHtmlTag content) {
        Tree.Add(new HtmlTag("div", Tree, content));
    }

    public void AddDiv(MailBodyBuilder bodyBuilder) {
        var div = new HtmlTag("div", Tree, null);
        foreach (IHtmlTag tag in bodyBuilder) {
            div.Add(tag);
        }
        Tree.Add(div);
    }
}