using _04GraphQLDocumentsWithDapper.Models;
using HotChocolate.Data.Filters;
using HotChocolate.Data.Projections.Context;
using HotChocolate.Resolvers;

namespace _04GraphQLDocumentsWithDapper;

public class Query
{
    [UseOffsetPaging]
    [UseSorting(typeof(DocumentSortInputType))]
    [UseFiltering(typeof(UserFilterType))]
    public async Task<IEnumerable<Document>> GetDocumentsAsync(
        [Service] DocumentRepository repository,
         IResolverContext resolver)
    {
        var selections = resolver.GetSelectedField();
        List<string> fields = new List<string>();
        foreach (var parent in selections.GetFields())
        {
            foreach (var innerCh in parent.GetFields())
            {
                fields.Add(innerCh.Field.Name);
            }
        }

        var filterDic = resolver.ArgumentValue<IReadOnlyDictionary<string, object>?>("where");

        string? orderBy = null;
        string? direction = "ASC";
        int? skip = null;
        int? take = null;

        return await repository.GetDocumentsAsync(orderBy, direction, skip, take);
    }

    public async Task<Document?> GetDocumentByIdAsync(
        [Service] DocumentRepository repository,
        int id)
    {
        return await repository.GetDocumentByIdAsync(id);
    }
}

public class UserFilterType : FilterInputType<Document>
{
    protected override void Configure(
        IFilterInputTypeDescriptor<Document> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(f => f.Title).Name("filter_title");
    }
}