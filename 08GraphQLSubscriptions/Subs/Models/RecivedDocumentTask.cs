namespace _08GraphQLSubscriptions.Subs.Models;

public class RecivedDocumentTask
{
    public RecivedDocumentTask(int documentId, string task, TimeOnly time)
    {
        DocumentId = documentId;
        Task = task;
        Time = time;
    }
    public int DocumentId { get; set; }
    public string Task { get; set; }
    public TimeOnly Time { get; set; }
}
