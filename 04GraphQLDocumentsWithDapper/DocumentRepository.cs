using _04GraphQLDocumentsWithDapper.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Data;
using Tag = _04GraphQLDocumentsWithDapper.Models.Tag;

namespace _04GraphQLDocumentsWithDapper;

public class DocumentRepository
{
    private readonly string _connectionString;

    public DocumentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public async Task<IEnumerable<Document>> GetDocumentsAsync(string? orderBy = null,
        string? direction = "ASC", int? skip = null, int? take = null)
    {
        using var connection = CreateConnection();

        // Base query
        var sql = "SELECT d.*, t.*, m.* FROM Documents d " +
                  "LEFT JOIN DocumentTags dt ON d.Id = dt.DocumentsId " +
                  "LEFT JOIN Tags t ON dt.TagsId = t.Id " +
                  "LEFT JOIN Metadata m ON d.Id = m.DocumentId";

        // Add ordering
        if (!string.IsNullOrEmpty(orderBy))
        {
            sql += $" ORDER BY d.{orderBy} {direction}";
        }
        else
        {
            sql += " ORDER BY d.Title ASC";
        }

        // Add pagination
        if (skip.HasValue && take.HasValue)
        {
            sql += $" LIMIT {take.Value} OFFSET {skip.Value}";
        }

        // Use Dapper's QueryAsync with a multi-mapping approach
        var documentDictionary = new Dictionary<int, Document>();

        await connection.QueryAsync<Document, Tag, Metadata, Document>(
            sql,
            (document, tag, metadata) =>
            {
                if (!documentDictionary.TryGetValue(document.Id, out var documentEntry))
                {
                    documentEntry = document;
                    documentEntry.Tags = new List<Tag>();
                    documentEntry.Metadata = new List<Metadata>();
                    documentDictionary.Add(document.Id, documentEntry);
                }

                if (tag != null && tag.Id != 0)
                {
                    if (!documentEntry.Tags.Any(t => t.Id == tag.Id))
                    {
                        documentEntry.Tags.Add(tag);
                    }
                }

                if (metadata != null && metadata.Id != 0)
                {
                    if (!documentEntry.Metadata.Any(m => m.Id == metadata.Id))
                    {
                        documentEntry.Metadata.Add(metadata);
                    }
                }

                return documentEntry;
            },
            splitOn: "Id,Id");

        return documentDictionary.Values;
    }

    public async Task<Document?> GetDocumentByIdAsync(int id)
    {
        using var connection = CreateConnection();

        var sql = "SELECT d.*, t.*, m.* FROM Documents d " +
                  "LEFT JOIN DocumentTags dt ON d.Id = dt.DocumentsId " +
                  "LEFT JOIN Tags t ON dt.TagsId = t.Id " +
                  "LEFT JOIN Metadata m ON d.Id = m.DocumentId " +
                  "WHERE d.Id = @Id";

        Document? result = null;
        await connection.QueryAsync<Document, Tag, Metadata, Document>(
            sql,
            (document, tag, metadata) =>
            {
                if (result == null)
                {
                    result = document;
                    result.Tags = new List<Tag>();
                    result.Metadata = new List<Metadata>();
                }

                if (tag != null && tag.Id != 0 && !result.Tags.Any(t => t.Id == tag.Id))
                {
                    result.Tags.Add(tag);
                }

                if (metadata != null && metadata.Id != 0 && !result.Metadata.Any(m => m.Id == metadata.Id))
                {
                    result.Metadata.Add(metadata);
                }

                return result;
            },
            new { Id = id },
            splitOn: "Id,Id");

        return result;
    }

    public async Task<int> CreateDocumentAsync(Document document)
    {
        using var connection = CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Insert document
            var sql = @"
                INSERT INTO Documents (Title, Content, Author, CreatedAt, ModifiedAt)
                VALUES (@Title, @Content, @Author, @CreatedAt, @ModifiedAt);
                SELECT last_insert_rowid();";

            var documentId = await connection.ExecuteScalarAsync<int>(sql, document, transaction);

            // Insert tags relationships
            if (document.Tags.Any())
            {
                foreach (var tag in document.Tags)
                {
                    // Insert tag if it doesn't exist
                    var existingTagId = await connection.ExecuteScalarAsync<int?>(
                        "SELECT Id FROM Tags WHERE Name = @Name", new { tag.Name }, transaction);

                    var tagId = existingTagId ?? await connection.ExecuteScalarAsync<int>(
                        "INSERT INTO Tags (Name) VALUES (@Name); SELECT last_insert_rowid();",
                        new { tag.Name }, transaction);

                    // Create relationship
                    await connection.ExecuteAsync(
                        "INSERT INTO DocumentTags (DocumentId, TagId) VALUES (@DocumentId, @TagId)",
                        new { DocumentId = documentId, TagId = tagId }, transaction);
                }
            }

            // Insert metadata
            if (document.Metadata.Any())
            {
                foreach (var meta in document.Metadata)
                {
                    await connection.ExecuteAsync(
                        "INSERT INTO Metadata (FieldName, Value, DocumentId) VALUES (@FieldName, @Value, @DocumentId)",
                        new { meta.FieldName, meta.Value, DocumentId = documentId }, transaction);
                }
            }

            transaction.Commit();
            return documentId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}