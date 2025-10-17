namespace Angular_Auth.Dto.Blogs;

public class BlogUpdate {
    public required Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? BlogContent { get; set; }
    public IFormFile? BannerImage  { get; set; }
}