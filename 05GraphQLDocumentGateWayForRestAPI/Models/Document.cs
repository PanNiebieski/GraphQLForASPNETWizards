namespace _05GraphQLDocumentGateWayForRestAPI.Models;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public string? Author { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public List<Tag> Tags { get; set; } = new();
    public List<Metadata> Metadata { get; set; } = new();
}