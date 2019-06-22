using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IGroupInviteRepository : IRepository<GroupInvite>
    {
    }

    public class GroupInviteRepository : Repository<GroupInvite, NaheulbookDbContext>, IGroupInviteRepository
    {
        public GroupInviteRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}