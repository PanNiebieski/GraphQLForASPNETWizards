namespace _08GraphQLSubscriptions.Subs.Models;

public class LLMAnswer
{
    public LLMAnswer(string answer, string question, int documentId)
    {
        Answer = answer;
        Question = question;
        DocumentId = documentId;
    }

    public string Answer { get; set; }
    public string Question { get; set; }
    public int DocumentId { get; set; }
}
