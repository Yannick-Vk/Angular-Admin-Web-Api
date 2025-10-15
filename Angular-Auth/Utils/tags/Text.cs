namespace Angular_Auth.Utils.tags;

public class Text(string text) : IHtmlTag {
    public string text { get; } = text;
    public bool IsBold { get; private set; }
    public bool IsItalic { get; private set; }

    public IHtmlTag Add(IHtmlTag child) {
        throw new ArgumentException("Cannot add to Text literal.", nameof(child));
    }

    public override string ToString() {
        var content = text;

        if (IsBold) content = new Bold(content).ToString();

        if (IsItalic) content = new Italic(content).ToString();

        return content;
    }

    public Text Bold() {
        IsBold = true;
        return this;
    }

    public Text Italic() {
        IsItalic = true;
        return this;
    }

    public IHtmlTag ToTag() {
        return this;
    }
}