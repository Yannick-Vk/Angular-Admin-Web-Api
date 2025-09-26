using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Angular_Auth.Dto;

namespace Angular_Auth.Models;

[Table("Blogs")]
public class Blog {
    public Blog() { }

    [SetsRequiredMembers]
    public Blog(BlogUpload upload, User author) {
        Id = Guid.NewGuid();
        Title = upload.Title.Trim();
        Description = upload.Description.Trim();
        CreatedAt = DateTime.Now;
        Authors = [author];
        BannerImage = upload.BannerImage;
    }

    public Guid Id { get; set; }

    [MaxLength(255)] public required string Title { get; set; }

    [MaxLength(255)] public required string Description { get; set; }

    public required List<User> Authors { get; set; } = [];
    [MaxLength(4096)] public required string BannerImage { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}