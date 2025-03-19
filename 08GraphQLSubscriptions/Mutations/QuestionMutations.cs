using _08GraphQLSubscriptions.Subs.Models;
using HotChocolate.Subscriptions;

namespace _08GraphQLSubscriptions.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class QuestionMutations
{
    [UseMutationConvention]
    public async Task<bool> SendQuestionAboutDocumentAsync(
        QuestionToLLMAboutDocument question,
          [Service] ITopicEventSender eventSender,
        CancellationToken cancellationToken)
    {
        await eventSender.SendAsync("questions", question);
        return true;
    }
}
