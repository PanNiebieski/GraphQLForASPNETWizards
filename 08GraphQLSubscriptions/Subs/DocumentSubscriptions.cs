using _08GraphQLSubscriptions.Subs.Models;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using System.Runtime.CompilerServices;

namespace _08GraphQLSubscriptions.Subs;

public class DocumentSubscriptions
{
    [Topic("questions")]
    [Subscribe]
    public LLMAnswer OnReciveQuestion(
        [EventMessage] QuestionToLLMAboutDocument question)
    {
        return new LLMAnswer("ANSWER", question.Question, question.DocumentId);
    }

    [Subscribe(With = nameof(OnDocumentTaskAsync))]
    public RecivedDocumentTask OnDocumentCreateTask(
        [EventMessage] DocumentTask documentTask,
        CancellationToken ct)
    {
        return new RecivedDocumentTask(documentTask.DocumentId,
            documentTask.Task,
            TimeOnly.FromDateTime(DateTime.Now));
    }

    public static async IAsyncEnumerable<DocumentTask> OnDocumentTaskAsync(
        [Service] AppDbContext dbContext,
        [Service] ITopicEventReceiver eventReceiver,
        [EnumeratorCancellation] CancellationToken ct)
    {
        // Subscribe to the correct topic that matches where DocumentTask objects are published
        ISourceStream<DocumentTask> sourceStream = await eventReceiver.SubscribeAsync<DocumentTask>(
            "DocumentTasked",
            ct);

        // Read events from the stream and yield them to subscribers
        await foreach (DocumentTask item in sourceStream.ReadEventsAsync().WithCancellation(ct))
        {
            if (item != null && !string.IsNullOrEmpty(item.Task) && item.DocumentId > 0)
            {
                yield return item;
            }
        }
    }
}