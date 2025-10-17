namespace Angular_Auth.Utils.tags;

public class Link(IHtmlTag content, string linkAddr) : HtmlTag("a", content) {
    public Link(string text, string linkAddr) : this(new Text(text), linkAddr) {
        LinkAddr = linkAddr;
    }

    public string LinkAddr { get; private set; }

    public override string ToString() {
        return $"<a href=\"{linkAddr}\">{Content}</a>";
    }
}