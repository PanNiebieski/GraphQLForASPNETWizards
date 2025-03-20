using _03GraphQLDocumentsSortingOrdering.Models;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace _03GraphQLDocumentsSortingOrdering;

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
        return resolver.ArgumentKind("order") is ValueKind.Null
        ? all.OrderBy(t => t.Title) : all;
    }
}

public class DocumentSortingInputType : SortInputType<Document>
{
    protected override void Configure(ISortInputTypeDescriptor<Document> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(t => t.Title);
    }
}