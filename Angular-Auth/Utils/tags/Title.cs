namespace Angular_Auth.Utils.tags;

public class TitleTag : HtmlTag {
    private IHtmlTag _content { get; set; }

    public TitleTag(IHtmlTag parent, IHtmlTag content, ushort level) : base("h", parent, content) {
        if (level > 6) throw new ArgumentException("Level cannot be bigger than 6.", nameof(level));
        Tag += level.ToString();
        _content = content;
    }

    public TitleTag(ushort level, IHtmlTag parent, string text) : this(parent, new Text(text), level) { }
}