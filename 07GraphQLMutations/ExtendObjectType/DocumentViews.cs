namespace _07GraphQLMutations.ExtendObjectType;

public class DocumentViews
{
    public DocumentViews(DateTime? fromDate, DateTime? toDate, int views)
    {
        FromDate = fromDate;
        ToDate = toDate;
        Views = views;
    }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Views { get; set; }
}

