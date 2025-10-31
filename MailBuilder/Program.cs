using Angular_Auth.Utils;

namespace MailBuilder;
internal static class Program {
    public static async Task Main(string[] args) {
        const string path = "../../../Mails";
        const string name = "demo";

        Console.WriteLine($"Creating Mail {name} in : '{path}'");

        var mail = new MailBodyBuilder()
            .AddTitle("Demo")
            .AddDiv(builder => builder
                .AddParagraph("This is a demo build")
                .AddLink("Link to GitHub", "https://github.com")
            );
        await mail.ToFiles(path, name);

        Console.WriteLine("Created Mail!");
    }
}