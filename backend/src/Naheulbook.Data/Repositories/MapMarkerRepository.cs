#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMapMarkerRepository : IRepository<MapMarkerEntity>
{
    Task<MapMarkerEntity?> GetWithLayerAsync(int mapMarkerId, CancellationToken cancellationToken = default);
    Task LoadLinksAsync(MapMarkerEntity mapMarker, CancellationToken cancellationToken = default);
}

public class MapMarkerRepository(NaheulbookDbContext context) : Repository<MapMarkerEntity, NaheulbookDbContext>(context), IMapMarkerRepository
{
    public Task<MapMarkerEntity?> GetWithLayerAsync(int mapMarkerId, CancellationToken cancellationToken = default)
    {
        return Context.MapMarkers
            .Include(e => e.Layer)
            .SingleOrDefaultAsync(e => e.Id == mapMarkerId, cancellationToken);
    }

    public Task LoadLinksAsync(MapMarkerEntity mapMarker, CancellationToken cancellationToken = default)
    {
        return Context.Entry(mapMarker)
            .Collection(x => x.Links)
            .Query()
            .Include(x => x.TargetMap)
            .LoadAsync(cancellationToken);
    }
}