using System.Text;

namespace Angular_Auth.Utils.tags;

public class Div : IHtmlTag {
    private List<IHtmlTag> _children { get; } = [];

    public IHtmlTag Add(IHtmlTag child) {
        _children.Add(child);
        return this;
    }

    public override string ToString() {
        var builder = new StringBuilder("<div>");

        foreach (var tag in _children) builder.Append(tag);

        builder.Append("</div>");
        return builder.ToString();
    }
}