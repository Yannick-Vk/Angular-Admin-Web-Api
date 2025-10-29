using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Angular_Auth.Dto.Blogs;

namespace Angular_Auth.Models;

[Table("Blogs")]
public class Blog {
    public Blog() { }

    [SetsRequiredMembers]
    public Blog(BlogUpload upload, User author) {
        Id = Guid.NewGuid();
        Title = upload.Title.Trim();
        Description = upload.Description.Trim();
        CreatedAt = DateTime.UtcNow;
        Authors = [author];
    }

    public Guid Id { get; set; }

    [MaxLength(255)] public required string Title { get; set; }

    [MaxLength(255)] public required string Description { get; set; }

    public required List<User> Authors { get; set; } = [];
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}