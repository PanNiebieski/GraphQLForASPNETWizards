namespace _04GraphQLDocumentsWithDapper.Models;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public string? Author { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties - These won't be populated automatically by Dapper
    // We'll need to write custom logic to populate them
    public List<Tag> Tags { get; set; } = new();
    public List<Metadata> Metadata { get; set; } = new();
}




