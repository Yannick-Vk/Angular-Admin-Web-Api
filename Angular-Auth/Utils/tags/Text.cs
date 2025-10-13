using System.Text;

namespace Angular_Auth.Utils.tags;

public class Text(string text) : IHtmlTag {
    public string text { get; private set; } = text;
    public bool IsBold { get; private set; }
    public bool IsItalic { get; private set; }

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
        var content = text;
        
        if (IsBold) {
            content = new Bold(this, content).ToString();
        }

        if (IsItalic) {
           content = new Italic(this, content).ToString(); 
        }
        
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