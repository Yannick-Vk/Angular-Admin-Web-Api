using System.Text;

namespace Angular_Auth.Utils;

public class HtmlTag(string tag, IHtmlTag parent) : IHtmlTag {
    public string Tag { get; protected set; } = tag;
    public IHtmlTag Parent { get; protected set; } = parent;
    public List<IHtmlTag> Children { get; protected set; } = [];

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
        var builder = new StringBuilder(Tag);

        foreach (var tag in Children) {
            builder.Append(tag.ToString());
        }
        
        return builder.ToString();
    }
}