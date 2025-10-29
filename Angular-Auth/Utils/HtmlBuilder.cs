using Angular_Auth.Utils.tags;

namespace Angular_Auth.Utils;

public class HtmlBuilder {
    public Root Tree { get; } = new();

    public string Build() {
        return Tree.ToString();
    }

    public void AddTitle(IHtmlComponent component, ushort level) {
        Tree.Add(new TitleTag(component, level));
    }

    public void AddTitle(Text text, ushort level) => AddTitle(text.ToComponent(), level);

    public void AddTitle(string title, ushort level) => AddTitle(new Text(title), level);

    public void AddParagraph(IHtmlComponent component) {
        Tree.Add(new Paragraph(component));
    }

    public void AddParagraph(Text text) => AddParagraph(text.ToComponent());

    public void AddParagraph(string text) => AddParagraph(new Text(text));

    public void AddLink(IHtmlComponent component, string link) {
        Tree.Add(new Link(component, link));
    }

    public void AddLink(Text text, string link) => AddLink(text.ToComponent(), link);
    public void AddLink(string text, string link) => AddLink(new Text(text), link);
}