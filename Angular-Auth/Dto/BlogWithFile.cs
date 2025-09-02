using Angular_Auth.Models;

namespace Angular_Auth.Dto;

public class BlogWithFile {
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string BlogContent { get; set; }
}