namespace _08GraphQLSubscriptions.Subs.Models;

public class DocumentTask
{
    public DocumentTask(int documentId, string task)
    {
        DocumentId = documentId;
        Task = task;
    }

    public int DocumentId { get; set; }
    public string Task { get; set; }
}