using System.Text;

namespace Angular_Auth.Utils;

public class HtmlTag(string tag, IHtmlTag parent, IHtmlTag? content) : IHtmlTag {
    public string Tag { get; protected set; } = tag;
    public IHtmlTag Parent { get; protected set; } = parent;
    public List<IHtmlTag> Children { get; protected set; } = [];

    public readonly IHtmlTag? Content = content;

    public IHtmlTag Add(IHtmlTag child) {
        if (child is HtmlTag tag) {
            tag.Parent = this;
        }

        Children.Add(child);
        return child;
    }

    public bool HasChildren() {
        return Children.Count > 0;
    }

    public IHtmlTag? LastChild() {
        return Children.LastOrDefault();
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.Append($"<{Tag}>");
        if (Content != null) {
            sb.Append(Content);
        }

        foreach (var child in Children) {
            sb.Append(child);
        }

        sb.Append($"</{Tag}>");
        return sb.ToString();
    }
}