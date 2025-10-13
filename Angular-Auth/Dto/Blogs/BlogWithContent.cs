using Angular_Auth.Dto.Users;
using Angular_Auth.Models;

namespace Angular_Auth.Dto.Blogs;

public class BlogWithContent {
    public BlogWithContent(Blog blog, string content) {
        Id = blog.Id;
        Title = blog.Title;
        Description = blog.Description;
        Authors = blog.Authors.Select(a => new UserDto(a)).ToList();
        CreatedAt = blog.CreatedAt;
        UpdatedAt = blog.UpdatedAt;
        Content = content;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<UserDto> Authors { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Content { get; set; }
}
