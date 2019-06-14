using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
    }

    public class GroupRepository : Repository<Group, NaheulbookDbContext>, IGroupRepository
    {
        public GroupRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}