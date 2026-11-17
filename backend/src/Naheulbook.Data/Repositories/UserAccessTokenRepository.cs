#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IUserAccessTokenRepository : IRepository<UserAccessTokenEntity>
{
    Task<List<UserAccessTokenEntity>> GetUserAccessTokensForUser(int userId, CancellationToken cancellationToken = default);
    Task<UserAccessTokenEntity?> GetByKeyAsync(string accessKey, CancellationToken cancellationToken = default);
    Task<UserAccessTokenEntity?> GetByUserIdAndTokenIdAsync(int userId, Guid userAccessTokenId, CancellationToken cancellationToken = default);
}

public class UserAccessTokenRepository(NaheulbookDbContext context) : Repository<UserAccessTokenEntity, NaheulbookDbContext>(context), IUserAccessTokenRepository
{
    public Task<List<UserAccessTokenEntity>> GetUserAccessTokensForUser(int userId, CancellationToken cancellationToken = default)
    {
        return Context.UserAccessTokens
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public Task<UserAccessTokenEntity?> GetByKeyAsync(string accessKey, CancellationToken cancellationToken = default)
    {
        return Context.UserAccessTokens
            .FirstOrDefaultAsync(x => x.Key == accessKey, cancellationToken);
    }

    public Task<UserAccessTokenEntity?> GetByUserIdAndTokenIdAsync(int userId, Guid userAccessTokenId, CancellationToken cancellationToken = default)
    {
        return Context.UserAccessTokens
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == userAccessTokenId, cancellationToken);
    }
}