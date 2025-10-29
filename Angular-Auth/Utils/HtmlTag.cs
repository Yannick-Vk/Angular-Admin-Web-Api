using System.Text;

namespace Angular_Auth.Utils;

public class HtmlTag(string tag, IHtmlComponent content) : IHtmlTag {
    public readonly IHtmlComponent Content = content;
    public string Tag { get; protected set; } = tag;
    public List<IHtmlTag> Children { get; } = [];

    public readonly HashSet<string> ClassSet = [];
    public List<string> ClassList => ClassSet.ToList();

    public IHtmlTag Add(IHtmlTag child) {
        Children.Add(child);
        return child;
    }

    private void _AddClass(string className) {
        ClassSet.Add(className.ToLower());
    }

    public IHtmlTag AddClass(string className) {
        _AddClass(className);
        return this;
    }

    public bool HasClass(string className) {
        return ClassSet.Contains(className.ToLower());
    }

    public IHtmlTag AddClass(string[] classes) {
        foreach (var className in classes) {
            _AddClass(className);
        }

        return this;
    }

    public override string ToString() {
        var builder = new StringBuilder($"<{Tag}>")
            .Append(Content);

        foreach (var child in Children) builder.Append(child);

        builder.Append($"</{Tag}>");
        return builder.ToString();
    }

    public IHtmlComponent ToComponent() {
        return this;
    }
}