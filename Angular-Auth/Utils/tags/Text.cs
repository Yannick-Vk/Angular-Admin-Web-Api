namespace Angular_Auth.Utils.tags;

public class Text(string text) : IHtmlTag {
    private string _text { get; } = text;
    public IHtmlTag Add(IHtmlTag child) {
        throw new ArgumentException("Cannot add to Text literal.", nameof(child));
    }

    public bool HasChildren() {
        return false;
    }

    public IHtmlTag? LastChild() {
        return null;
    }

    public override string ToString() {
        return _text;
    }
}