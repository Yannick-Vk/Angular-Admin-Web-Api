using Angular_Auth.Utils.tags;

namespace Angular_Auth.Utils;

public class HtmlBuilder {
    public Root Tree { get; private set; }= new();

    public string Build() {
        return Tree.ToString();
    }

    public void AddTitle(Text text, ushort level) => AddTitle(text.ToTag(), level); 

    public void AddTitle(IHtmlTag tag, ushort level) {
        Tree.Add(new TitleTag(Tree, tag, level));
    }

    public void AddParagraph(IHtmlTag tag) {
        Tree.Add(new Paragraph(Tree, tag));
    }

    public void AddParagraph(Text text) => AddParagraph(text.ToTag());

    public void AddLink(Text text, string link) => AddLink(text.ToTag(), link);

    public void AddLink(IHtmlTag tag, string link) {
        Tree.Add(new Link(Tree, tag, link));
    }

    public void AddDiv(IHtmlTag content) {
        Tree.Add(new HtmlTag("div", Tree, content));
    }
}