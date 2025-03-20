namespace _03GraphQLDocumentsSortingOrdering.Models;

public class Metadata
{
    public int Id { get; set; }
    public string FieldName { get; set; }
    public string? Value { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;
}