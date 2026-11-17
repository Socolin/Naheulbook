using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMapMarkerLinkRepository : IRepository<MapMarkerLinkEntity>
{
    Task<MapMarkerLinkEntity?> GetWithLayerAsync(int mapMarkerLinkId, CancellationToken cancellationToken = default);
    Task LoadTargetMapAsync(MapMarkerLinkEntity mapMarkerLink, CancellationToken cancellationToken = default);
}

public class MapMarkerLinkRepository(NaheulbookDbContext context) : Repository<MapMarkerLinkEntity, NaheulbookDbContext>(context), IMapMarkerLinkRepository
{
    public Task<MapMarkerLinkEntity?> GetWithLayerAsync(int mapMarkerLinkId, CancellationToken cancellationToken = default)
    {
        return Context.MapMarkerLinks
            .Include(x => x.MapMarker)
            .ThenInclude(x => x.Layer)
            .FirstOrDefaultAsync(x => x.Id == mapMarkerLinkId, cancellationToken);
    }

    public Task LoadTargetMapAsync(MapMarkerLinkEntity mapMarkerLink, CancellationToken cancellationToken = default)
    {
        return Context.Entry(mapMarkerLink)
            .Reference(x => x.TargetMap)
            .LoadAsync(cancellationToken);
    }
}