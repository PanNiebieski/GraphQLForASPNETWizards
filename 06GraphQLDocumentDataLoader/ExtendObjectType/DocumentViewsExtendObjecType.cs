using _06GraphQLDocumentDataLoader.Models;
using System.Text.Json;

namespace _06GraphQLDocumentDataLoader.ExtendObjectType;

[ExtendObjectType(typeof(Document))]
public class DocumentViewsExtendObjecType
{

    public static async Task<DocumentViews> GetViewChange(ChangeSpan span,
     [Parent] Document parent,
     ViewsByKeyFrom_Hotchocolate_Types_Analyzers_DataLoader loader,
     CancellationToken cancellationToken)
    {
        return await loader.LoadAsync(new KeyAndSpan(parent.Id, span), cancellationToken);
    }


    [DataLoader(name: "ViewsByKeyFrom_Hotchocolate_Types_Analyzers_")]
    public static async Task<IReadOnlyDictionary<KeyAndSpan, DocumentViews>> GetViewsByKey(
        IReadOnlyList<KeyAndSpan> keysAndSpans,
        [Service] IHttpClientFactory clientFactory,
        CancellationToken cancellationToken)
    {

        using HttpClient client = clientFactory.CreateClient("documentViews");
        var map = new Dictionary<KeyAndSpan, DocumentViews>();

        foreach (var span in keysAndSpans.GroupBy(t => t.Span))
        {
            using var message = new HttpRequestMessage
                 (HttpMethod.Get, $"/views/span/{span.Key.ToString().ToLower()}");

            using var response = await client.SendAsync(message, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var json = JsonDocument.Parse(content);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var list = json.Deserialize<List<DocumentViewRecord>>(options);

            foreach (var recordByDocumentId in list.GroupBy(a => a.DocumentId))
            {
                var sublist = recordByDocumentId.Select(a => a).ToList();

                var documentViews = new DocumentViews(sublist.GetDateFirst(),
                    sublist.GetDateLast(), sublist.Sum(a => a.Views));
                var keyAndSpan = new KeyAndSpan(recordByDocumentId.Key, span.Key);

                map.Add(keyAndSpan, documentViews);
            }
        }

        return map;
    }

    //private static int NumberOfRequests { get; set; }

    //// The loader is now injected as the concrete type.
    //public async Task<DocumentViews> GetViewChange(
    //    ChangeSpan span,
    //    [Parent] Document parent,
    //    ViewsByKeyDataLoader2 loader,
    //    CancellationToken cancellationToken)
    //{
    //    NumberOfRequests++;

    //    return await loader.LoadAsync(new KeyAndSpan(parent.Id, span), cancellationToken);
    //}

    //This will create TestDataLoader
    [DataLoader(name: "Test")]
    public static async Task<Dictionary<int, string>> GetTestIdAsync(
    IReadOnlyList<int> ids,
    CancellationToken ct)
    => new Dictionary<int, string>() { { 1, "1" } };

}
