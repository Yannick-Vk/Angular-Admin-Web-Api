namespace Angular_Auth.Utils.tags;

public class Link(IHtmlTag content, string linkAddr) : HtmlTag("a", content) {
    public Link(string text, string linkAddr) : this(new Text(text), linkAddr) {
        _linkAddr = linkAddr;
    }

    private string _linkAddr { get; set; } = linkAddr;

    public override string ToString() {
        return $"<a href=\"{_linkAddr}\">{Content}</a>";
    }
}