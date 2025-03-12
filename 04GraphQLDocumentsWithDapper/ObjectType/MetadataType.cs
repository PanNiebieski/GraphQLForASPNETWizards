using _04GraphQLDocumentsWithDapper.Models;
using HotChocolate.Types;

namespace _04GraphQLDocumentsWithDapper.ObjectType;

public class MetadataType : ObjectType<Metadata>
{
    protected override void Configure(IObjectTypeDescriptor<Metadata> descriptor)
    {
        descriptor.Description("Represents metadata associated with a document");

        descriptor.Field(m => m.Id).Description("The unique identifier of the metadata");
        descriptor.Field(m => m.FieldName).Description("The field name of the metadata");
        descriptor.Field(m => m.Value).Description("The value of the metadata");
        descriptor.Field(m => m.DocumentId).Description("The document this metadata is associated with");
    }
}