using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapRepository : IRepository<Map>
    {
    }

    public class MapRepository : Repository<Map, NaheulbookDbContext>, IMapRepository
    {
        public MapRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}