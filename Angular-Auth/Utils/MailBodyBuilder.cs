using System.Collections;
using Angular_Auth.Utils.tags;

namespace Angular_Auth.Utils;

public class MailBodyBuilder : IEnumerable {
    private readonly HtmlBuilder _htmlBuilder = new();
    private readonly List<string> _text = [];

    public IEnumerator GetEnumerator() {
        return _htmlBuilder.Tree.Children.GetEnumerator();
    }

    public (string html, string text) Build() {
        return (_htmlBuilder.Build(), GetText());
    }

    private string GetText() {
        return GetText(_text);
    }

    private static string GetText(List<string> text) {
        return string.Join("\n", text);
    }

    public MailBodyBuilder AddTitle(string title, ushort level = 1) {
        return AddTitle(new Text(title), level);
    }

    public MailBodyBuilder AddTitle(Text text, ushort level = 1) {
        if (level == 1) {
            // If its null replace it with the title
            _htmlBuilder.Tree.Title ??= text.text;
        }
        else {
            _add_text("");
        }

        _add_text(text);
        _htmlBuilder.AddTitle(text, level);
        return this;
    }

    public MailBodyBuilder AddParagraph(Text text) {
        _add_text(text);
        _htmlBuilder.AddParagraph(text);
        return this;
    }

    public MailBodyBuilder AddParagraph(string text) {
        return AddParagraph(new Text(text));
    }

    private void _add_text(string text) {
        _text.Add(text);
    }

    private void _add_text(Text text) {
        _text.Add(text.text);
    }

    public MailBodyBuilder AddLink(Text text, string link) {
        _add_text(text + " '" + link + "'");
        _htmlBuilder.AddLink(text, link);
        return this;
    }

    public MailBodyBuilder AddLink(string text, string link) {
        return AddLink(new Text(text), link);
    }

    public MailBodyBuilder AddDiv(Action<MailBodyBuilder> builderAction) {
        var divBuilder = new MailBodyBuilder();
        builderAction(divBuilder);

        _add_text(divBuilder.GetText());
        var div = new Div();
        foreach (var child in divBuilder._htmlBuilder.Tree.Children) {
            div.Add(child);
        }

        _htmlBuilder.Tree.Add(div);

        return this;
    }

    public async Task<MailBodyBuilder> ToFiles(string path, string name) {
        var (html, text) = Build();
        await CreateAndWriteToFile(path, name, "html", html);
        await CreateAndWriteToFile(path, name, ".txt", text);
        return this;
    }

    private static async Task CreateAndWriteToFile(string path, string name, string extension, string data) {
        if (extension.StartsWith('.')) {
            extension = extension[1..];
        }

        Directory.CreateDirectory($"{path}/{name}");
        await using var fs = File.Create($"{path}/{name}/{name}.{extension}");
        await using var output = new StreamWriter(fs);
        await output.WriteAsync(data);
    }
}