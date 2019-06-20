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
    }
}