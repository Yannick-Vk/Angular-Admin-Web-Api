using System.Diagnostics.CodeAnalysis;
using Angular_Auth.Models;

namespace Angular_Auth.Dto;

public class BlogWithFile {
    public BlogWithFile() { }

    [SetsRequiredMembers]
    public BlogWithFile(Blog blog, string fileContent) {
        Id = blog.Id;
        Title = blog.Title;
        Description = blog.Description;
        Authors = blog.Authors.Select(user => user.UserName?? "[NULL]").ToList();
        CreatedAt = blog.CreatedAt;
        UpdatedAt = blog.UpdatedAt;
        BlogContent = fileContent;
        BannerImage = blog.BannerImage;
    }

    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string BlogContent { get; set; }
    public required List<string> Authors { get; set; } = [];
    public required string BannerImage { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}