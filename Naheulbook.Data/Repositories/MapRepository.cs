using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapRepository : IRepository<Map>
    {
        Task<Map> GetMapDetailsForCurrentUserAsync(int mapId, int? userId);
    }

    public class MapRepository : Repository<Map, NaheulbookDbContext>, IMapRepository
    {
        public MapRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public async Task<Map> GetMapDetailsForCurrentUserAsync(int mapId, int? userId)
        {
            var map = await Context.Maps
                .Where(m => m.Id == mapId)
                .SingleAsync();

            map.Layers = await Context.MapLayers
                .Where(x => x.MapId == mapId)
                .Where(x => x.Source == "official" || (x.Source == "private" && x.UserId == userId))
                .Include(x => x.Markers)
                .ThenInclude(x => x.Links)
                .ThenInclude(x => x.TargetMap)
                .ToListAsync();

            return map;
        }
    }
}