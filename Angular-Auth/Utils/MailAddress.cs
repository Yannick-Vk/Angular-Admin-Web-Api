using System.Diagnostics.CodeAnalysis;

namespace Angular_Auth.Utils;

public class MailAddress {
    public required string name { get; init; }
    public required string address { get; init; }

    [SetsRequiredMembers]
    public MailAddress(string name, string address) {
        this.name = name;
        this.address = address;
    }

    [SetsRequiredMembers]
    public MailAddress((string name, string address) email) : this(email.name, email.address) { }
    
    public (string name, string address) ToTuple() => (name, address);

    public static (string name, string address) ToTuple(string name, string address) => (name, address);
}