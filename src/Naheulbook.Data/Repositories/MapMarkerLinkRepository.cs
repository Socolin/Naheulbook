using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IMapMarkerLinkRepository : IRepository<MapMarkerLinkEntity>
{
    Task<MapMarkerLinkEntity> GetWithLayerAsync(int mapMarkerLinkId);
    Task LoadTargetMapAsync(MapMarkerLinkEntity mapMarkerLink);
}

public class MapMarkerLinkRepository : Repository<MapMarkerLinkEntity, NaheulbookDbContext>, IMapMarkerLinkRepository
{
    public MapMarkerLinkRepository(NaheulbookDbContext context)
        : base(context)
    {
    }

    public Task<MapMarkerLinkEntity> GetWithLayerAsync(int mapMarkerLinkId)
    {
        return Context.MapMarkerLinks
            .Include(x => x.MapMarker)
            .ThenInclude(x => x.Layer)
            .FirstOrDefaultAsync(x => x.Id == mapMarkerLinkId);
    }

    public Task LoadTargetMapAsync(MapMarkerLinkEntity mapMarkerLink)
    {
        return Context.Entry(mapMarkerLink)
            .Reference(x => x.TargetMap)
            .LoadAsync();
    }
}