using System.Text.Json;

namespace _06GraphQLDocumentDataLoader.ExtendObjectType;

public class ViewsByKeyDataLoader : BatchDataLoader<KeyAndSpan, DocumentViews>
{
    private readonly IHttpClientFactory _clientFactory;

    public ViewsByKeyDataLoader(
        IBatchScheduler batchScheduler,
        IHttpClientFactory clientFactory,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _clientFactory = clientFactory;
    }

    private int NumberOfRequests { get; set; }

    protected override async Task<IReadOnlyDictionary<KeyAndSpan, DocumentViews>> LoadBatchAsync(
        IReadOnlyList<KeyAndSpan> keysAndSpans,
        CancellationToken cancellationToken)
    {
        NumberOfRequests++;

        using HttpClient client = _clientFactory.CreateClient("documentViews");

        var map = new Dictionary<KeyAndSpan, DocumentViews>();

        foreach (var group in keysAndSpans.GroupBy(k => k.Span))
        {
            // Prepare the request for the current span.
            using var message = new HttpRequestMessage(
                HttpMethod.Get, $"/views/span/{group.Key.ToString().ToLower()}");

            using var response = await client.SendAsync(message, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            using var json = JsonDocument.Parse(content);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var list = json.Deserialize<List<DocumentViewRecord>>(options);

            foreach (var recordGroup in list.GroupBy(r => r.DocumentId))
            {
                var sublist = recordGroup.ToList();

                var documentViews = new DocumentViews(
                    sublist.GetDateFirst(),
                    sublist.GetDateLast(),
                    sublist.Sum(a => a.Views));

                var key = new KeyAndSpan(recordGroup.Key, group.Key);
                map.Add(key, documentViews);
            }
        }

        return map;
    }
}