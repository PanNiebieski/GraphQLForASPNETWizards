namespace _04GraphQLDocumentsWithDapper.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    // Navigation property - will need custom logic to populate
    public List<Document> Documents { get; set; } = new();
}