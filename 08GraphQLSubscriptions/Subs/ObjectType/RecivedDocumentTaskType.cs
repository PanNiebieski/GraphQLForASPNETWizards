using _08GraphQLSubscriptions.Subs.Models;

namespace _08GraphQLSubscriptions.Subs.ObjectType;

public class RecivedDocumentTaskType : ObjectType<RecivedDocumentTask>
{
    protected override void Configure(IObjectTypeDescriptor<RecivedDocumentTask> descriptor)
    {
        descriptor.Field(t => t.DocumentId).Description("The document ID");
        descriptor.Field(t => t.Task).Description("The task");
        descriptor.Field(t => t.Time).Description("The time the task was created");

    }
}
