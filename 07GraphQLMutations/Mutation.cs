using _07GraphQLMutations.Models;
using Microsoft.EntityFrameworkCore;

namespace _07GraphQLMutations;


public class Mutation
{
    [Error<NotAuthenticatedException>]
    [Error<DocumentDoesntExist>]
    [Error<UserDoesNotExistException>]
    [UseMutationConvention]
    public async Task<Watchlist> AddDocumentToWatchlistAsync(
        [Service] WatchlistRepository repository,
        int documentId,
        string? userName,
        CancellationToken cancellationToken)
    {
        return await repository.AddDocumentToWatchlistAsync(documentId, userName, cancellationToken);
    }

    [Error<NotAuthenticatedException>]
    [Error<DocumentDoesntExist>]
    [Error<UserDoesNotExistException>]
    [Error<WatchlistItemDoesNotExistException>]
    [UseMutationConvention]
    public async Task<bool> RemoveDocumentFromWatchlistAsync(
        [Service] WatchlistRepository repository,
        int documentId,
        string? userName,
        CancellationToken cancellationToken)
    {
        return await repository.RemoveDocumentFromWatchlistAsync(documentId, userName, cancellationToken);
    }

}

public class WatchlistRepository
{
    private readonly AppDbContext _dbContext;

    public WatchlistRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Watchlist> AddDocumentToWatchlistAsync(
        int documentId, string? userName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userName))
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
            throw new UserDoesNotExistException();
        }

        // Check if the document is already in the watchlist for this user
        var existingWatchlist = await _dbContext.Watchlists
            .FirstOrDefaultAsync(w => w.DocumentId == documentId && w.UserId == userId.Value, cancellationToken);

        if (existingWatchlist != null)
        {
            return existingWatchlist; // Already in watchlist, return existing
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

    public async Task<bool> RemoveDocumentFromWatchlistAsync(
        int documentId, string? userName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userName))
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
            throw new UserDoesNotExistException();
        }

        var watchlistItem = await _dbContext.Watchlists
            .FirstOrDefaultAsync(w => w.DocumentId == documentId && w.UserId == userId.Value, cancellationToken);

        if (watchlistItem == null)
        {
            throw new WatchlistItemDoesNotExistException();
        }

        _dbContext.Watchlists.Remove(watchlistItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

}

// Updated exception classes with consistent naming
public class NotAuthenticatedException : Exception
{
    public NotAuthenticatedException() : base("User is not authenticated") { }
}

public class UserDoesNotExistException : Exception
{
    public UserDoesNotExistException() : base("User does not exist") { }
}

public class DocumentDoesntExist : Exception
{
    public DocumentDoesntExist() : base("Document does not exist") { }
}

public class WatchlistItemDoesNotExistException : Exception
{
    public WatchlistItemDoesNotExistException() : base("Watchlist item does not exist") { }
}

public static class Users
{
    public static int? GetId(string name)
    {
        return name switch
        {
            "cezary" => 1,
            "john" => 2,
            _ => null
        };
    }
}
