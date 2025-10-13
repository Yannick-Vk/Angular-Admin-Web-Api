namespace Angular_Auth.Utils.tags;

public class Link(IHtmlTag parent, IHtmlTag content, string linkAddr) : HtmlTag("a", parent, content) {
    
    public string LinkAddr { get; private set; }

    public Link(IHtmlTag parent, string text, string linkAddr) : this(parent, new Text(text), linkAddr) {
        LinkAddr = linkAddr;
    }

    public override string ToString() {
        return $"<a href=\"{linkAddr}\">{Content}</a>";
    }
}