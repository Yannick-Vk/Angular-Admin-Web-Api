using System.Text;

namespace Angular_Auth.Utils;

public class HtmlTag(string tag, IHtmlTag parent, IHtmlTag content) : IHtmlTag {
    public string Tag { get; protected set; } = tag;
    public IHtmlTag Parent { get; protected set; } = parent;
    public List<IHtmlTag> Children { get; protected set; } = [];

    public readonly IHtmlTag Content = content;

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
        var builder = new StringBuilder($"<{Tag}>")
            .Append(content); 

        foreach (var child in Children) {
            builder.Append(child);
        }

        builder.Append($"</{Tag}>");
        return builder.ToString();
    }
}