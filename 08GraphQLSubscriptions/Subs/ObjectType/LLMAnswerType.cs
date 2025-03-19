using _08GraphQLSubscriptions.Subs.Models;

namespace _08GraphQLSubscriptions.Subs.ObjectType;

public class LLMAnswerType : ObjectType<LLMAnswer>
{
    protected override void Configure(IObjectTypeDescriptor<LLMAnswer> descriptor)
    {
        descriptor.Field(t => t.Answer).Description("The answer to the question");
        descriptor.Field(t => t.Question).Description("The question");
        descriptor.Field(t => t.DocumentId).Description("The document ID");

    }
}