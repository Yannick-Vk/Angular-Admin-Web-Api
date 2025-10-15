using System.Text;

namespace Angular_Auth.Utils.tags;

public class Div : IHtmlTag {
    public List<IHtmlTag> Children { get; } = new();

    public IHtmlTag Add(IHtmlTag child) {
        Children.Add(child);
        return this;
    }

    public bool HasChildren() {
        return Children.Count > 0;
    }

    public IHtmlTag? LastChild() {
        return Children.LastOrDefault();
    }

    public override string ToString() {
        var builder = new StringBuilder("<div>");

        foreach (var tag in Children) {
            builder.Append(tag);
        }

        builder.Append("</div>");
        return builder.ToString();
    }
}