namespace Angular_Auth.Dto;

public class BlogUpdate {
    public required Guid Id {get; set;}
    public required string Title { get; set; }
    public required string Description { get; set; }
}