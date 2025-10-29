using Angular_Auth.Utils;

internal class Program {
    public static async Task Main(string[] args) {
        var path = "../../../Mails";
        var name = "demo";

        Console.WriteLine($"Creating Mail {name} in : '{path}'");

        var mail = new MailBodyBuilder()
            .AddTitle("Demo")
            .AddDiv(builder => builder
                .AddParagraph("This is a demo build")
                .AddLink("Link to github", "https://github.com")
            );
        await mail.ToFiles(path, name);

        Console.WriteLine("Created Mail!");
    }
}