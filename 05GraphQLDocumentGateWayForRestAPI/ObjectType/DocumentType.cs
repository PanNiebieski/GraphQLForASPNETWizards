using _05GraphQLDocumentGateWayForRestAPI.Models;

namespace _05GraphQLDocumentGateWayForRestAPI;

public class DocumentType : ObjectType<Document>
{
    protected override void Configure(IObjectTypeDescriptor<Document> descriptor)
    {
        descriptor.Description("Represents a document in the system");

        // This line explicitly configures the id field to NOT use global id encoding:
        descriptor.Field(d => d.Id)
                  .ID("Document")
                  .Description("The unique identifier of the document");

        descriptor.Field(d => d.Title).Description("The title of the document");
        descriptor.Field(d => d.Content).Description("The content of the document");
        descriptor.Field(d => d.Author).Description("The author of the document");
        descriptor.Field(d => d.Tags).Description("The tags associated with the document");
        descriptor.Field(d => d.Metadata).Description("The metadata associated with the document");
    }
}