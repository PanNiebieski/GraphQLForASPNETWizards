namespace _03GraphQLDocumentsSortingOrdering.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Document> Documents { get; set; } = new();
}