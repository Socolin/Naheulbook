using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapMarkerLinkRepository : IRepository<MapMarkerLink>
    {
    }

    public class MapMarkerLinkRepository : Repository<MapMarkerLink, NaheulbookDbContext>, IMapMarkerLinkRepository
    {
        public MapMarkerLinkRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}