#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByFacebookIdAsync(string facebookId, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default);
    Task<UserEntity?> GetByMicrosoftIdAsync(string microsoftId, CancellationToken cancellationToken = default);
    Task<List<UserEntity>> SearchUsersAsync(string filter, CancellationToken cancellationToken = default);
}

public class UserRepository(NaheulbookDbContext naheulbookDbContext) : Repository<UserEntity, NaheulbookDbContext>(naheulbookDbContext), IUserRepository
{
    public Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return Context.Users
            .Where(u => u.Username == username)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<UserEntity?> GetByFacebookIdAsync(string facebookId, CancellationToken cancellationToken = default)
    {
        return Context.Users
            .Where(u => u.FbId == facebookId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<UserEntity?> GetByGoogleIdAsync(string googleId, CancellationToken cancellationToken = default)
    {
        return Context.Users
            .Where(u => u.GoogleId == googleId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<UserEntity?> GetByMicrosoftIdAsync(string microsoftId, CancellationToken cancellationToken = default)
    {
        return Context.Users
            .Where(u => u.MicrosoftId == microsoftId)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<List<UserEntity>> SearchUsersAsync(string filter, CancellationToken cancellationToken = default)
    {
        return Context.Users
            .Where(u => u.DisplayName != null && u.DisplayName.ToLower().Contains(filter.ToLower()))
            .Where(u => u.ShowInSearchUntil > DateTime.UtcNow)
            .Take(10)
            .ToListAsync(cancellationToken);
    }
}