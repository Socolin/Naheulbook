using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMapMarkerLinkRepository : IRepository<MapMarkerLinkEntity>
{
    Task<MapMarkerLinkEntity?> GetWithLayerAsync(int mapMarkerLinkId);
    Task LoadTargetMapAsync(MapMarkerLinkEntity mapMarkerLink);
}

public class MapMarkerLinkRepository(NaheulbookDbContext context) : Repository<MapMarkerLinkEntity, NaheulbookDbContext>(context), IMapMarkerLinkRepository
{
    public Task<MapMarkerLinkEntity?> GetWithLayerAsync(int mapMarkerLinkId)
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