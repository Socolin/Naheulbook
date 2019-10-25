using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapMarkerRepository : IRepository<MapMarker>
    {
    }

    public class MapMarkerRepository : Repository<MapMarker, NaheulbookDbContext>, IMapMarkerRepository
    {
        public MapMarkerRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}