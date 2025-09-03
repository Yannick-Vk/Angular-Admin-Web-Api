namespace Angular_Auth.Dto;

public class BlogUpdate {
    public required Guid Id {get; set;}
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? UpdatedFileContent { get; set; }
}