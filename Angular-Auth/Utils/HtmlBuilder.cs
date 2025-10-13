using Angular_Auth.Utils.tags;

namespace Angular_Auth.Utils;

public class HtmlBuilder {
    private readonly Root _tree = new();

    public string Build() {
        return _tree.ToString();
    }

    public void AddTitle(Text text, ushort level) {
        _tree.Add(new TitleTag(_tree, text, level));
    }

    public void AddParagraph(Text text) {
        _tree.Add(new Paragraph(_tree, text));
    }

    public void AddLink(Text text, string link) {
        _tree.Add(new Link(_tree, text, link));
    }
}