using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IStatRepository : IRepository<Stat>
    {
    }

    public class StatRepository : Repository<Stat, NaheulbookDbContext>, IStatRepository
    {
        public StatRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}