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
    }
}