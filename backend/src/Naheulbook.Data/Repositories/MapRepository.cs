using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMapRepository : IRepository<MapEntity>
{
    Task<MapEntity?> GetMapDetailsForCurrentUserAsync(int mapId, int? userId);
}

public class MapRepository(NaheulbookDbContext context) : Repository<MapEntity, NaheulbookDbContext>(context), IMapRepository
{
    public async Task<MapEntity?> GetMapDetailsForCurrentUserAsync(int mapId, int? userId)
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