
using _03GraphQLDocumentsSortingOrdering.Models;
using HotChocolate.Types;

namespace _03GraphQLDocumentsSortingOrdering;

public class DocumentType : ObjectType<Document>
{
    protected override void Configure(IObjectTypeDescriptor<Document> descriptor)
    {
        descriptor.Description("Represents a document in the system");

        descriptor.Field(d => d.Id).Description("The unique identifier of the document");
        descriptor.Field(d => d.Title).Description("The title of the document");
        descriptor.Field(d => d.Content).Description("The content of the document");
        descriptor.Field(d => d.Author).Description("The author of the document");
        //descriptor.Field(d => d.CreatedAt).Description("When the document was created");
        //descriptor.Field(d => d.ModifiedAt).Description("When the document was last modified");
        descriptor.Field(d => d.Tags).Description("The tags associated with the document");
        descriptor.Field(d => d.Metadata).Description("The metadata associated with the document");
    }
}