namespace Angular_Auth.Models;

public class Blog {
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
}