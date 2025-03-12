using Microsoft.EntityFrameworkCore;

namespace _02GraphQLDocumentsAndMetaData.Models;

public class Document
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Content { get; set; }
    public string? Author { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public List<Tag> Tags { get; set; } = new();
    public List<Metadata> Metadata { get; set; } = new();
}




