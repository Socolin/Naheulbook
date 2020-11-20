#pragma warning disable 8619
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IUserAccessTokenRepository : IRepository<UserAccessToken>
    {
        Task<List<UserAccessToken>> GetUserAccessTokensForUser(int userId);
        Task<UserAccessToken?> GetByKeyAsync(string accessKey);
        Task<UserAccessToken?> GetByUserIdAndTokenIdAsync(int userId, Guid userAccessTokenId);
    }

    public class UserAccessTokenRepository : Repository<UserAccessToken, NaheulbookDbContext>, IUserAccessTokenRepository
    {
        public UserAccessTokenRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<UserAccessToken>> GetUserAccessTokensForUser(int userId)
        {
            return Context.UserAccessTokens
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public Task<UserAccessToken?> GetByKeyAsync(string accessKey)
        {
            return Context.UserAccessTokens
                .FirstOrDefaultAsync(x => x.Key == accessKey);
        }

        public Task<UserAccessToken?> GetByUserIdAndTokenIdAsync(int userId, Guid userAccessTokenId)
        {
            return Context.UserAccessTokens
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == userAccessTokenId);
        }
    }
}