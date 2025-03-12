using System.ComponentModel.DataAnnotations;

namespace _05RestAPIDocumentViews;

public class DocumentView
{
    [Key]
    public int Id { get; set; }
    public string DocumentId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int Views { get; set; }
}