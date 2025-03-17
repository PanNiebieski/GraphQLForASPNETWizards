using _05GraphQLDocumentGateWayForRestAPI.Models;
using HotChocolate.Data.Sorting;
using HotChocolate.Resolvers;
using Microsoft.EntityFrameworkCore;

namespace _05GraphQLDocumentGateWayForRestAPI;

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

public class DocumentSortingInputType : SortInputType<Document>
{
    protected override void Configure(ISortInputTypeDescriptor<Document> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(t => t.Title);
    }
}