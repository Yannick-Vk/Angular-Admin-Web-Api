using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Angular_Auth.Models;

[Table("Blogs")]
public class Blog {
    public Guid Id { get; set; }
    [MaxLength(255)]
    public required string Title { get; set; }
    [MaxLength(9500)]
    public required string Description { get; set; }
    public required User Author { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}