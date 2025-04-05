#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IUserAccessTokenRepository : IRepository<UserAccessTokenEntity>
{
    Task<List<UserAccessTokenEntity>> GetUserAccessTokensForUser(int userId);
    Task<UserAccessTokenEntity?> GetByKeyAsync(string accessKey);
    Task<UserAccessTokenEntity?> GetByUserIdAndTokenIdAsync(int userId, Guid userAccessTokenId);
}

public class UserAccessTokenRepository(NaheulbookDbContext context) : Repository<UserAccessTokenEntity, NaheulbookDbContext>(context), IUserAccessTokenRepository
{
    public Task<List<UserAccessTokenEntity>> GetUserAccessTokensForUser(int userId)
    {
        return Context.UserAccessTokens
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public Task<UserAccessTokenEntity?> GetByKeyAsync(string accessKey)
    {
        return Context.UserAccessTokens
            .FirstOrDefaultAsync(x => x.Key == accessKey);
    }

    public Task<UserAccessTokenEntity?> GetByUserIdAndTokenIdAsync(int userId, Guid userAccessTokenId)
    {
        return Context.UserAccessTokens
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == userAccessTokenId);
    }
}