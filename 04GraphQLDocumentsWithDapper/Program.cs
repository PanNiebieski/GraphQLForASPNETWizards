using _04GraphQLDocumentsWithDapper;
using _04GraphQLDocumentsWithDapper.Models;
using _04GraphQLDocumentsWithDapper.ObjectType;
using HotChocolate.Data.Sorting;
using SQLitePCL;

Batteries.Init();

var builder = WebApplication.CreateBuilder(args);

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=documents.db";

// Register services
builder.Services.AddSingleton(new DocumentRepository(connectionString));

// Register GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddType<DocumentType>()
    .AddType<TagType>()
    .AddType<MetadataType>()
    .AddSorting().AddFiltering();
;

var app = builder.Build();

app.MapGraphQL();

app.Run();


public class DocumentSortInputType : SortInputType<Document>
{
    protected override void Configure(ISortInputTypeDescriptor<Document> descriptor)
    {
        descriptor.BindFieldsExplicitly();
        descriptor.Field(d => d.Id);
    }
}