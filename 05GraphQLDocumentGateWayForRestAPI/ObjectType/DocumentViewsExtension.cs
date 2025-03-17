
using _05GraphQLDocumentGateWayForRestAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace _05GraphQLDocumentGateWayForRestAPI.ObjectType;

[Node]
[ExtendObjectType(typeof(Document))]
public sealed class DocumentViewsExtension
{
    public async Task<List<DocumentViewStatistics>> GetViewChange(ChangeSpan span,
        [Parent] Document parent,
        [Service] IHttpClientFactory clientFactory,
        CancellationToken cancellationToken)
    {
        using HttpClient client = clientFactory.CreateClient("documentViews");

        using var message = new HttpRequestMessage
            (HttpMethod.Get, $"/views/{parent.Id}/{span.ToString().ToLower()}");

        var response = await client.SendAsync(message, cancellationToken);
        var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var json = JsonDocument.Parse(content);

        //return json.RootElement;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var a = json.Deserialize<List<DocumentViewStatistics>>(options);

        return a;
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

public enum ChangeSpan
{
    Day,
    Week,
    Month
}

