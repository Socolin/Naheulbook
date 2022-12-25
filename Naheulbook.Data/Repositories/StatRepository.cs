using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IStatRepository : IRepository<StatEntity>
    {
    }

    public class StatRepository : Repository<StatEntity, NaheulbookDbContext>, IStatRepository
    {
        public StatRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}