using System.Text;

namespace Angular_Auth.Utils.tags;

public class Root : IHtmlTag {
    public string Tag { get; protected set; } = "root";
    public List<IHtmlTag> Children { get; protected set; } = [];

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
        var builder = new StringBuilder("<html><body>");

        foreach (var tag in Children) {
            builder.Append(tag);
        }

        builder.Append("</body></html>");
        return builder.ToString();
    }
}