namespace _9GraphQLDemoOfDeferAndStream.QueriesAndTypes;

public class Review
{
    public int Id { get; set; }
    public string AuthorName { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
