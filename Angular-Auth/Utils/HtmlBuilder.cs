using Angular_Auth.Utils.tags;

namespace Angular_Auth.Utils;

public class HtmlBuilder {
    private readonly Root _tree = new();

    public string Build() {
        return _tree.ToString();
    }

    public HtmlBuilder AddTitle(string text, ushort level) {
        var last = _tree.Children.LastOrDefault();
        if (last is null) {
            _tree.Add(new TitleTag(level, _tree, text));
            return this;
        }
        
        _addLast(new TitleTag(level, last, text), last);
        return this;
    }

    private HtmlBuilder _addLast(IHtmlTag tag, IHtmlTag last) {
        var child = last.LastChild();
        if (child is not null) {
            return _addLast(tag, child);
        }

        last.Add(tag);
        return this;
    }
}