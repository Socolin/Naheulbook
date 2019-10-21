using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapLayerRepository : IRepository<MapLayer>
    {
    }

    public class MapLayerRepository : Repository<MapLayer, NaheulbookDbContext>, IMapLayerRepository
    {
        public MapLayerRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}