using System.Text;

namespace Angular_Auth.Utils;

public class HtmlTag(string tag, IHtmlTag content) : IHtmlTag {
    public readonly IHtmlTag Content = content;
    public string Tag { get; protected set; } = tag;
    public List<IHtmlTag> Children { get; } = [];

    public IHtmlTag Add(IHtmlTag child) {
        Children.Add(child);
        return child;
    }

    public override string ToString() {
        var builder = new StringBuilder($"<{Tag}>")
            .Append(Content);

        foreach (var child in Children) builder.Append(child);

        builder.Append($"</{Tag}>");
        return builder.ToString();
    }
}