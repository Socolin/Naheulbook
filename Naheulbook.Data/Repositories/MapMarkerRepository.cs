using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapMarkerRepository : IRepository<MapMarker>
    {
        Task<MapMarker> GetWithLayerAsync(int mapMarkerId);
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
    }
}