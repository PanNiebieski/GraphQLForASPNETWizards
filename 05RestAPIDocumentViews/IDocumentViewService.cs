namespace _05RestAPIDocumentViews;

public interface IDocumentViewService
{
    Task<IEnumerable<DocumentViewStatistics>> GetDocumentViewsOverTimeAsync(string documentId, TimeFrame timeFrame);
}
