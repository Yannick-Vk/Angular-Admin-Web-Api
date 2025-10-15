using Angular_Auth.Utils.tags;

namespace Angular_Auth.Utils;

public class HtmlBuilder {
    public Root Tree { get; } = new();

    public string Build() {
        return Tree.ToString();
    }

    public void AddTitle(Text text, ushort level) {
        AddTitle(text.ToTag(), level);
    }

    public void AddTitle(IHtmlTag tag, ushort level) {
        Tree.Add(new TitleTag(tag, level));
    }

    public void AddParagraph(IHtmlTag tag) {
        Tree.Add(new Paragraph(tag));
    }

    public void AddParagraph(Text text) {
        AddParagraph(text.ToTag());
    }

    public void AddLink(Text text, string link) {
        AddLink(text.ToTag(), link);
    }

    public void AddLink(IHtmlTag tag, string link) {
        Tree.Add(new Link(tag, link));
    }
}