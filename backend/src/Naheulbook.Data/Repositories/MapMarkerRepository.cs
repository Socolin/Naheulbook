#pragma warning disable 8619
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IMapMarkerRepository : IRepository<MapMarkerEntity>
{
    Task<MapMarkerEntity?> GetWithLayerAsync(int mapMarkerId);
    Task LoadLinksAsync(MapMarkerEntity mapMarker);
}

public class MapMarkerRepository(NaheulbookDbContext context) : Repository<MapMarkerEntity, NaheulbookDbContext>(context), IMapMarkerRepository
{
    public Task<MapMarkerEntity?> GetWithLayerAsync(int mapMarkerId)
    {
        return Context.MapMarkers
            .Include(e => e.Layer)
            .SingleOrDefaultAsync(e => e.Id == mapMarkerId);
    }

    public Task LoadLinksAsync(MapMarkerEntity mapMarker)
    {
        return Context.Entry(mapMarker)
            .Collection(x => x.Links)
            .Query()
            .Include(x => x.TargetMap)
            .LoadAsync();
    }
}