using _07GraphQLMutations.Models;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace _07GraphQLMutations;

public class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Document> GetDocuments(AppDbContext context,
        IResolverContext resolver)
    {
        var all = context.Documents.Include(a => a.Tags).Include(a => a.Metadata);
        return all;
    }

}
