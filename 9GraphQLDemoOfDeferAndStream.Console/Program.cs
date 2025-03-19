using _9GraphQLDemoOfDeferAndStream.QueriesAndTypes;
using _9GraphQLDemoOfDeferAndStream.QueriesAndTypes.ObjectType;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

var services = new ServiceCollection();

services.AddSingleton<ProductResolvers>();

services.AddGraphQLServer()
    .ModifyOptions(o =>
    {
        o.EnableDefer = true;
        o.EnableStream = true;
    }).AddQueryType<Query>()
            .AddType<ProductType>()
            .AddType<ProductDetailsType>()
            .AddType<ReviewType>();


var serviceProvider = services.BuildServiceProvider();

var executorResolver = serviceProvider.GetRequiredService<IRequestExecutorResolver>();
var executor = await executorResolver.GetRequestExecutorAsync();

Console.WriteLine("Running GraphQL query with @defer and @stream...");

//Client query: the @defer directive tells the server to return the 'details'
// field later, and the @stream directive streams each review as it becomes available.
var query = @"
query GetProduct {
  product(id: 1) {
    id
    name
    price
    description
    ...DeferredDetails @defer
    reviews @stream {
      id
      authorName
      rating
      comment
      createdAt
    }
  }
}

fragment DeferredDetails on Product {
  details {
    manufacturer
    manufactureDate
    countryOfOrigin
    materialInfo
    dimensions
    technicalSpecifications
  }
}
";

var finalResult = new Dictionary<string, object>();

var result = await executor.ExecuteAsync(query);

if (result is IResponseStream responseStream)
{
    await foreach (var incrementalResult in responseStream.ReadResultsAsync())
    {
        Console.WriteLine($"Received update at {DateTime.Now:HH:mm:ss}");

        var json = incrementalResult.ToJson();
        Console.WriteLine(json);

        UpdateFinalJson(finalResult, json);
    }

    WriteFinalJson(finalResult);
}
else
{
    // In case the result isn't incremental, just output the full JSON.
    Console.WriteLine();
}

Console.WriteLine("\nQuery execution completed.");


/// <summary>
/// Recursively merges dictionary source into target.
/// </summary>
void MergeDictionaries(Dictionary<string, object> target, Dictionary<string, object> source)
{
    foreach (var kv in source)
    {
        if (kv.Value is Dictionary<string, object> srcDict)
        {
            if (target.TryGetValue(kv.Key, out var existing) && existing is Dictionary<string, object> targetDict)
            {
                MergeDictionaries(targetDict, srcDict);
            }
            else
            {
                target[kv.Key] = srcDict;
            }
        }
        else if (kv.Value is List<object> srcList)
        {
            // For arrays, simply override (or you can merge element-by-element if needed)
            target[kv.Key] = srcList;
        }
        else
        {
            target[kv.Key] = kv.Value;
        }
    }
}

/// <summary>
/// Applies an incremental patch (with a "path" and "data" or "items") to the target dictionary.
/// The patch path is relative to the "data" object.
/// </summary>
void ApplyIncrementalPatch(Dictionary<string, object> target, JsonElement incremental)
{
    if (!incremental.TryGetProperty("path", out var pathElement))
        return;

    var path = new List<object>();
    foreach (var item in pathElement.EnumerateArray())
    {
        if (item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out int index))
        {
            path.Add(index);
        }
        else if (item.ValueKind == JsonValueKind.String)
        {
            path.Add(item.GetString());
        }
    }

    if (incremental.TryGetProperty("data", out var dataElement) && dataElement.ValueKind != JsonValueKind.Null)
    {
        MergeAtPath(target, path, dataElement);
    }
    else if (incremental.TryGetProperty("items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
    {
        // Assume a single item in "items" array.
        var enumerator = itemsElement.EnumerateArray();
        if (enumerator.MoveNext())
        {
            SetAtPath(target, path, enumerator.Current);
        }
    }
}

/// <summary>
/// Recursively merges a JSON patch (patchData) into the object at the given path.
/// </summary>
void MergeAtPath(object currentObj, List<object> path, JsonElement patchData)
{
    // Base case: no more segments, merge patchData into currentObj if it is a dictionary.
    if (path.Count == 0)
    {
        if (currentObj is Dictionary<string, object> dict)
        {
            foreach (var prop in patchData.EnumerateObject())
            {
                dict[prop.Name] = JsonElementToObject(prop.Value);
            }
        }
        return;
    }

    var segment = path[0];
    path.RemoveAt(0);

    if (segment is string key)
    {
        if (currentObj is Dictionary<string, object> dict)
        {
            if (!dict.TryGetValue(key, out var next))
            {
                // Look ahead: if the next segment is an int, create a list; else create a dictionary.
                next = (path.Count > 0 && path[0] is int)
                    ? (object)new List<object>()
                    : new Dictionary<string, object>();
                dict[key] = next;
            }
            MergeAtPath(next, path, patchData);
        }
        else
        {
            throw new Exception("Expected a dictionary for a string path segment.");
        }
    }
    else if (segment is int index)
    {
        if (currentObj is List<object> list)
        {
            // Ensure the list is large enough.
            while (list.Count <= index)
            {
                list.Add(null);
            }
            if (path.Count > 0)
            {
                if (list[index] == null)
                {
                    // Look ahead: if the next segment is an int, create a list; else a dictionary.
                    list[index] = (path[0] is int)
                        ? (object)new List<object>()
                        : new Dictionary<string, object>();
                }
                MergeAtPath(list[index], path, patchData);
            }
            else
            {
                // Final segment: assign the value.
                list[index] = JsonElementToObject(patchData);
            }
        }
        else
        {
            throw new Exception("Expected a list at an integer path segment.");
        }
    }
}

/// <summary>
/// Converts a JsonElement to a .NET object recursively (Dictionary, List, or primitive).
/// </summary>
object JsonElementToObject(JsonElement element)
{
    switch (element.ValueKind)
    {
        case JsonValueKind.Object:
            var dict = new Dictionary<string, object>();
            foreach (var prop in element.EnumerateObject())
            {
                dict[prop.Name] = JsonElementToObject(prop.Value);
            }
            return dict;
        case JsonValueKind.Array:
            var list = new List<object>();
            foreach (var item in element.EnumerateArray())
            {
                list.Add(JsonElementToObject(item));
            }
            return list;
        case JsonValueKind.String:
            return element.GetString();
        case JsonValueKind.Number:
            if (element.TryGetInt64(out long l))
                return l;
            return element.GetDouble();
        case JsonValueKind.True:
        case JsonValueKind.False:
            return element.GetBoolean();
        case JsonValueKind.Null:
            return null;
        default:
            return element.ToString();
    }
}


/// <summary>
/// Sets a value at the given path in the target object.
/// </summary>
void SetAtPath(Dictionary<string, object> current, List<object> path, JsonElement value)
{
    if (path.Count == 0)
        return;

    var segment = path[0];
    if (path.Count == 1)
    {
        if (segment is string key)
        {
            current[key] = JsonElementToObject(value);
        }
        else if (segment is int)
        {
            throw new Exception("Expected a dictionary but found an integer key.");
        }
        return;
    }

    if (segment is string keySegment)
    {
        if (!current.TryGetValue(keySegment, out var next))
        {
            // Decide whether to create a list or dictionary based on the next path segment.
            next = path[1] is int ? (object)new List<object>() : new Dictionary<string, object>();
            current[keySegment] = next;
        }
        if (next is Dictionary<string, object> nextDict)
        {
            path.RemoveAt(0);
            SetAtPath(nextDict, path, value);
        }
        else if (next is List<object> nextList)
        {
            path.RemoveAt(0);
            SetAtPathInList(nextList, path, value);
        }
    }
}

/// <summary>
/// Helper method for setting a value in a list given a path.
/// </summary>
void SetAtPathInList(List<object> list, List<object> path, JsonElement value)
{
    if (path.Count == 0)
        return;

    if (!(path[0] is int index))
        throw new Exception("Expected an integer index in list path.");

    if (path.Count == 1)
    {
        while (list.Count <= index)
            list.Add(null);
        list[index] = JsonElementToObject(value);
        return;
    }

    while (list.Count <= index)
        list.Add(null);

    if (list[index] == null)
    {
        list[index] = path[1] is int ? (object)new List<object>() : new Dictionary<string, object>();
    }

    if (list[index] is Dictionary<string, object> dict)
    {
        path.RemoveAt(0);
        SetAtPath(dict, path, value);
    }
    else if (list[index] is List<object> innerList)
    {
        path.RemoveAt(0);
        SetAtPathInList(innerList, path, value);
    }
}



void UpdateFinalJson(Dictionary<string, object> finalResult, string json)
{
    using (var doc = JsonDocument.Parse(json))
    {
        var root = doc.RootElement;

        // Merge the top-level "data" object into finalResult.
        if (root.TryGetProperty("data", out var dataProp))
        {
            var dataObj = JsonElementToObject(dataProp) as Dictionary<string, object>;
            if (!finalResult.ContainsKey("data"))
            {
                finalResult["data"] = dataObj;
            }
            else
            {
                MergeDictionaries(finalResult["data"] as Dictionary<string, object>, dataObj);
            }
        }

        // Process any incremental patch entries.
        if (root.TryGetProperty("incremental", out var incrementalProp) && incrementalProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var inc in incrementalProp.EnumerateArray())
            {
                // Each patch has a "path" (an array of keys/indexes)
                // and either "data" (an object patch) or "items" (for arrays).
                ApplyIncrementalPatch(finalResult["data"] as Dictionary<string, object>, inc);
            }
        }
    }
}

static void WriteFinalJson(Dictionary<string, object> finalResult)
{
    var finalJson = JsonSerializer.Serialize(finalResult, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine("\n========Final Merged JSON=========\n");
    Console.WriteLine(finalJson);
}