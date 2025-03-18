using _07GraphQLMutations.Models;
using Microsoft.EntityFrameworkCore;

namespace _07GraphQLMutations;

[MutationType]
public class WatchlistMutation
{
    [Error<NotAuthenticatedException>]
    [Error<DocumentDoesntExist>]
    [Error<UserDontExistException>]
    [Mutation]
    public async Task<Watchlist> AddDocumentToWatchlistAsync(
        [Service] WatchlistRepository respo,
        int documentId,
        string? userName,
        CancellationToken cancellationToken)
    {
        return await respo.AddDocumentToWtchlistAsync(documentId, userName, cancellationToken);
    }
}

public class WatchlistRepository
{
    private readonly AppDbContext _dbContext;

    public WatchlistRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Watchlist> AddDocumentToWtchlistAsync(
        int documentId, string? userName, CancellationToken cancellationToken)
    {
        if (userName is null)
        {
            throw new NotAuthenticatedException();
        }

        if (!await _dbContext.Documents.AnyAsync(d => d.Id == documentId, cancellationToken))
        {
            throw new DocumentDoesntExist();
        }

        var userId = Users.GetId(userName.ToLower());

        if (userId is null)
        {
            throw new UserDontExistException();
        }

        var watchlist = new Watchlist
        {
            DocumentId = documentId,
            UserId = userId.Value,
        };
        _dbContext.Watchlists.Add(watchlist);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return watchlist;
    }
}

public class NotAuthenticatedException : Exception
{
    public NotAuthenticatedException() : base("User is not authenticated") { }
}

public class UserDontExistException : Exception
{
    public UserDontExistException() : base("User dont exist") { }
}


public class DocumentDoesntExist : Exception
{
    public DocumentDoesntExist() : base("Document does not exist") { }
}

public static class Users
{
    public static int? GetId(string name)
    {
        if (name == "cezary")
            return 1;
        else if (name == "john")
            return 2;
        else
            return null;
    }
}

