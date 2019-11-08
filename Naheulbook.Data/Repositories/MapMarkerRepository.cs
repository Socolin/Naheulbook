using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapMarkerRepository : IRepository<MapMarker>
    {
        Task<MapMarker> GetWithLayerAsync(int mapMarkerId);
        Task LoadLinksAsync(MapMarker mapMarker);
    }

    public class MapMarkerRepository : Repository<MapMarker, NaheulbookDbContext>, IMapMarkerRepository
    {
        public MapMarkerRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<MapMarker> GetWithLayerAsync(int mapMarkerId)
        {
            return Context.MapMarkers
                .Include(e => e.Layer)
                .SingleOrDefaultAsync(e => e.Id == mapMarkerId);
        }

        public Task LoadLinksAsync(MapMarker mapMarker)
        {
            return Context.Entry(mapMarker)
                .Collection(x => x.Links)
                .Query()
                .Include(x => x.TargetMap)
                .LoadAsync();
        }
    }
}