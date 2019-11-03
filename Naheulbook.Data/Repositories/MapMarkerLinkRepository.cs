using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapMarkerLinkRepository : IRepository<MapMarkerLink>
    {
        Task<MapMarkerLink> GetWithLayerAsync(int mapMarkerLinkId);
        Task LoadTargetMapAsync(MapMarkerLink mapMarkerLink);
    }

    public class MapMarkerLinkRepository : Repository<MapMarkerLink, NaheulbookDbContext>, IMapMarkerLinkRepository
    {
        public MapMarkerLinkRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<MapMarkerLink> GetWithLayerAsync(int mapMarkerLinkId)
        {
            return Context.MapMarkerLinks
                .Include(x => x.MapMarker)
                .ThenInclude(x => x.Layer)
                .FirstOrDefaultAsync(x => x.Id == mapMarkerLinkId);
        }

        public Task LoadTargetMapAsync(MapMarkerLink mapMarkerLink)
        {
            return Context.Entry(mapMarkerLink)
                .Reference(x => x.TargetMap)
                .LoadAsync();
        }
    }
}