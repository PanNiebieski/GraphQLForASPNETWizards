using _02GraphQLDocumentsAndMetaData.Models;
using Microsoft.EntityFrameworkCore;

namespace _02GraphQLDocumentsAndMetaData;

public class Query
{
    public IQueryable<Document> GetDocuments(AppDbContext context)
    {
        return context.Documents.Include(a => a.Tags).Include(a => a.Metadata);
    }
}