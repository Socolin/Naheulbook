using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IGodRepository : IRepository<GodEntity>
    {
    }

    public class GodRepository : Repository<GodEntity, NaheulbookDbContext>, IGodRepository
    {
        public GodRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}