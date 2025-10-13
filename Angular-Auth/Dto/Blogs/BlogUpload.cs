namespace Angular_Auth.Dto.Blogs;

public class BlogUpload {
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string File { get; set; }
    public IFormFile? BannerImage { get; set; }
}