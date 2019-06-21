using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByFacebookIdAsync(string facebookId);
        Task<User> GetByGoogleIdAsync(string googleId);
        Task<User> GetByTwitterIdAsync(string twitterId);
    }

    public class UserRepository : Repository<User, NaheulbookDbContext>, IUserRepository
    {
        public UserRepository(NaheulbookDbContext naheulbookDbContext)
            : base(naheulbookDbContext)
        {
        }

        public Task<User> GetByUsernameAsync(string username)
        {
            return Context.Users
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();
        }

        public Task<User> GetByFacebookIdAsync(string facebookId)
        {
            return Context.Users
                .Where(u => u.FbId == facebookId)
                .SingleOrDefaultAsync();
        }

        public Task<User> GetByGoogleIdAsync(string googleId)
        {
            return Context.Users
                .Where(u => u.GoogleId == googleId)
                .SingleOrDefaultAsync();
        }

        public Task<User> GetByTwitterIdAsync(string twitterId)
        {
            return Context.Users
                .Where(u => u.TwitterId == twitterId)
                .SingleOrDefaultAsync();
        }
    }
}