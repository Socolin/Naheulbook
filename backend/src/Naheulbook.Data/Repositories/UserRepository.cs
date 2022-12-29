#pragma warning disable 8619
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<UserEntity?> GetByUsernameAsync(string username);
    Task<UserEntity?> GetByFacebookIdAsync(string facebookId);
    Task<UserEntity?> GetByGoogleIdAsync(string googleId);
    Task<UserEntity?> GetByTwitterIdAsync(string twitterId);
    Task<UserEntity?> GetByMicrosoftIdAsync(string microsoftId);
    Task<List<UserEntity>> SearchUsersAsync(string filter);
}

public class UserRepository : Repository<UserEntity, NaheulbookDbContext>, IUserRepository
{
    public UserRepository(NaheulbookDbContext naheulbookDbContext)
        : base(naheulbookDbContext)
    {
    }

    public Task<UserEntity?> GetByUsernameAsync(string username)
    {
        return Context.Users
            .Where(u => u.Username == username)
            .SingleOrDefaultAsync();
    }

    public Task<UserEntity?> GetByFacebookIdAsync(string facebookId)
    {
        return Context.Users
            .Where(u => u.FbId == facebookId)
            .SingleOrDefaultAsync();
    }

    public Task<UserEntity?> GetByGoogleIdAsync(string googleId)
    {
        return Context.Users
            .Where(u => u.GoogleId == googleId)
            .SingleOrDefaultAsync();
    }

    public Task<UserEntity?> GetByTwitterIdAsync(string twitterId)
    {
        return Context.Users
            .Where(u => u.TwitterId == twitterId)
            .SingleOrDefaultAsync();
    }

    public Task<UserEntity?> GetByMicrosoftIdAsync(string microsoftId)
    {
        return Context.Users
            .Where(u => u.MicrosoftId == microsoftId)
            .SingleOrDefaultAsync();
    }

    public Task<List<UserEntity>> SearchUsersAsync(string filter)
    {
        return Context.Users
            .Where(u => u.DisplayName != null && u.DisplayName.ToLower().Contains(filter.ToLower()))
            .Where(u => u.ShowInSearchUntil > DateTime.UtcNow)
            .Take(10)
            .ToListAsync();
    }
}