﻿using _03GraphQLDocumentsSortingOrdering.Models;
using Tag = _03GraphQLDocumentsSortingOrdering.Models.Tag;

namespace _03GraphQLDocumentsSortingOrdering;

public class TagType : ObjectType<Tag>
{
    protected override void Configure(IObjectTypeDescriptor<Tag> descriptor)
    {
        descriptor.Description("Represents a tag that can be associated with documents");

        descriptor.Field(t => t.Id).Description("The unique identifier of the tag");
        descriptor.Field(t => t.Name).Description("The name of the tag");
    }
}