using System.Diagnostics.CodeAnalysis;
using Angular_Auth.Models;

namespace Angular_Auth.Dto;

public class BlogWithAuthor {
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Author { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public BlogWithAuthor() { }

    [SetsRequiredMembers]
    public BlogWithAuthor(Blog blog) {
        Title = blog.Title;
        Description = blog.Description;
        Author = blog.Author.UserName ?? "NULL USER";
        CreatedAt = blog.CreatedAt;
        UpdatedAt = blog.UpdatedAt;
    }
}