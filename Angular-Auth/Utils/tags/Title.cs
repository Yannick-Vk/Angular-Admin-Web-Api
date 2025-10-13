namespace Angular_Auth.Utils.tags;

public class TitleTag : HtmlTag {
    private IHtmlTag _content { get; set; }

    public TitleTag(ushort level, IHtmlTag parent, IHtmlTag content) : base("h", parent) {
        if (level > 6) throw new ArgumentException("Level cannot be bigger than 6.", nameof(level));
        Tag += level.ToString();
        _content = content;
    }

    public TitleTag(ushort level, IHtmlTag parent, string text) : this(level, parent, new Text(text)) { }

    public override string ToString() {
        return $"<{Tag}>{_content}</{Tag}>";
    }
}