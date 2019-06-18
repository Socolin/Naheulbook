using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IGodRepository : IRepository<God>
    {
    }

    public class GodRepository : Repository<God, NaheulbookDbContext>, IGodRepository
    {
        public GodRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}