using MimeKit;

namespace Angular_Auth.Dto;

public class SendMailDto {
    public required string ToUsername { get; init; }
    public required string ToEmail { get; init; }
    public required string Subject { get; init; }
    public required BodyBuilder Body { get; init; } 
    
    
}