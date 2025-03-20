namespace _08GraphQLSubscriptions.Subs.Models;

public class QuestionToLLMAboutDocument
{
    public QuestionToLLMAboutDocument(int documentId, string question)
    {
        DocumentId = documentId;
        Question = question;
    }

    public int DocumentId { get; set; }
    public string Question { get; set; }
}