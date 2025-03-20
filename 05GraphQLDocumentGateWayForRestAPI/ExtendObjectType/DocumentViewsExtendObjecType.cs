using _05GraphQLDocumentGateWayForRestAPI.Models;
using System.Text.Json;

namespace _05GraphQLDocumentGateWayForRestAPI.ExtendObjectType;

//[Node]
[ExtendObjectType(typeof(Document))]
public sealed class DocumentViewsExtendObjecType
{
    public async Task<DocumentViews> GetViewChange(ChangeSpan span,
        [Parent] Document parent,
        [Service] IHttpClientFactory clientFactory,
        CancellationToken cancellationToken)
    {
        using HttpClient client = clientFactory.CreateClient("documentViews");

        using var message = new HttpRequestMessage
            (HttpMethod.Get, $"/views/id/{parent.Id}/span/{span.ToString().ToLower()}");

        var response = await client.SendAsync(message, cancellationToken);
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var json = JsonDocument.Parse(content);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var list = json.Deserialize<List<DocumentViewRecord>>(options);

        if (list == null || list.Count == 0)
        {
            return new DocumentViews(null, null, 0);
        }

        return new DocumentViews(list.GetDateFirst(),
            list.GetDateLast(), list.Sum(a => a.Views));
    }

    //[NodeResolver]
    //public static async Task<Document> GetDocumentByIdAsync(
    //[ID] int id,
    //[Service] AppDbContext context,
    //CancellationToken cancellationToken)
    //{
    //    return await context.Documents.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    //}
}