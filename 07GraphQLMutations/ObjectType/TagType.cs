using Tag = _07GraphQLMutations.Models.Tag;

namespace _07GraphQLMutations;

public class TagType : ObjectType<Tag>
{
    protected override void Configure(IObjectTypeDescriptor<Tag> descriptor)
    {
        descriptor.Description("Represents a tag that can be associated with documents");

        descriptor.Field(t => t.Id).Description("The unique identifier of the tag");
        descriptor.Field(t => t.Name).Description("The name of the tag");
    }
}