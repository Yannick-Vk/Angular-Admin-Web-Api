using System.Text;

namespace Angular_Auth.Utils.tags;

public class Root(string lang = "en", string charSet = "UTF-8") : IHtmlTag {
    public string? Title { get; set; }
    public List<IHtmlTag> Children { get; } = [];
    public HashSet<string> ClassSet { get; } = [];

    public IHtmlTag Add(IHtmlTag child) {
        Children.Add(child);
        return this;
    }

    private void _AddClass(string className) => ClassSet.Add(className.ToLower());

    public IHtmlTag AddClass(string className) {
        _AddClass(className);
        return this;
    }

    public IHtmlTag AddClass(string[] classes) {
        foreach (string className in classes) {
            _AddClass(className);
        }

        return this;
    }

    public bool HasClass(string className) {
        return ClassSet.Contains(className);
    }

    public override string ToString() {
        var builder = new StringBuilder($"""
                                         <!DOCTYPE html>
                                         <html lang="{lang}">
                                         <head>
                                             <meta charset="{charSet}">
                                             <title>{Title}</title>
                                         </head>
                                         <body>
                                         """
        );

        foreach (var tag in Children) builder.Append(tag);

        builder.Append("</body></html>");
        return builder.ToString();
    }

    public IHtmlComponent ToComponent() {
        return this;
    }
}