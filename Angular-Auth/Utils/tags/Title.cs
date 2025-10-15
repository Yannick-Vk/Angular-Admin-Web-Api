namespace Angular_Auth.Utils.tags;

public class TitleTag : HtmlTag {
    public TitleTag(IHtmlTag content, ushort level) : base("h", content) {
        if (level > 6) throw new ArgumentException("Level cannot be bigger than 6.", nameof(level));
        Level = level;
        Tag += level.ToString();
        _content = content;
    }

    public TitleTag(ushort level, string text) : this(new Text(text), level) { }
    private IHtmlTag _content { get; set; }
    public ushort Level { get; private set; }
}