using _08GraphQLSubscriptions.Models;
using Microsoft.EntityFrameworkCore;


namespace _08GraphQLSubscriptions.Mutations;

public class Mutation
{
    [Error<NotAuthenticatedException>]
    [Error<DocumentDoesntExist>]
    [Error<UserDoesNotExistException>]
    [Error<WrongPasswordException>]
    [UseMutationConvention]
    public async Task<Watchlist> AddDocumentToWatchlistAsync(
        [Service] WatchlistRepository repository,
        int documentId,
        [GlobalStateUserName] string? userName,
        [GlobalStatePassword] string? password,
        CancellationToken cancellationToken)
    {
        int userId = CheckUser(userName, password);
        return await repository.AddDocumentToWatchlistAsync(documentId, userId, cancellationToken);
    }

    [Error<NotAuthenticatedException>]
    [Error<DocumentDoesntExist>]
    [Error<UserDoesNotExistException>]
    [Error<WatchlistItemDoesNotExistException>]
    [Error<WrongPasswordException>]
    [UseMutationConvention]
    public async Task<bool> RemoveDocumentFromWatchlistAsync(
        [Service] WatchlistRepository repository,
        int documentId,
        [GlobalStateUserName] string? userName,
        [GlobalStatePassword] string? password,
        CancellationToken cancellationToken)
    {
        int userId = CheckUser(userName, password);
        return await repository.RemoveDocumentFromWatchlistAsync(documentId, userId, cancellationToken);
    }

    private int CheckUser(string? userName, string? password)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            throw new NotAuthenticatedException();
        }

        if (password != "password")
        {
            throw new WrongPasswordException();
        }

        var userId = Users.GetId(userName.ToLower());

        if (userId is null)
        {
            throw new UserDoesNotExistException();
        }

        return userId.Value;
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
        int documentId, int userId, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Documents.AnyAsync(d => d.Id == documentId, cancellationToken))
        {
            throw new DocumentDoesntExist();
        }

        // Check if the document is already in the watchlist for this user
        var existingWatchlist = await _dbContext.Watchlists
            .FirstOrDefaultAsync(w => w.DocumentId == documentId && w.UserId == userId, cancellationToken);

        if (existingWatchlist != null)
        {
            return existingWatchlist; // Already in watchlist, return existing
        }

        var watchlist = new Watchlist
        {
            DocumentId = documentId,
            UserId = userId,
        };

        _dbContext.Watchlists.Add(watchlist);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return watchlist;
    }

    public async Task<bool> RemoveDocumentFromWatchlistAsync(
        int documentId, int userId, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Documents.AnyAsync(d => d.Id == documentId, cancellationToken))
        {
            throw new DocumentDoesntExist();
        }

        var watchlistItem = await _dbContext.Watchlists
            .FirstOrDefaultAsync(w => w.DocumentId == documentId && w.UserId == userId, cancellationToken);

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

public class WrongPasswordException : Exception
{
    public WrongPasswordException() : base("Wrong password") { }
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
