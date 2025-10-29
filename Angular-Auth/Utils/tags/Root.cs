using System.Text;

namespace Angular_Auth.Utils.tags;

public class Root(string lang = "en", string charSet = "UTF-8") : IHtmlTag {
    public string Tag { get; protected set; } = "root";
    public string? Title { get; set; }
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
}