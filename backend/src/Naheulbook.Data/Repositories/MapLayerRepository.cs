using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IMapLayerRepository : IRepository<MapLayerEntity>
{
    Task LoadMarkersForResponseAsync(MapLayerEntity mapLayer);
}

public class MapLayerRepository(NaheulbookDbContext context) : Repository<MapLayerEntity, NaheulbookDbContext>(context), IMapLayerRepository
{
    public Task LoadMarkersForResponseAsync(MapLayerEntity mapLayer)
    {
        return Context.Entry(mapLayer)
            .Collection(x => x.Markers)
            .Query()
            .Include(x => x.Links)
            .ThenInclude(x => x.TargetMap)
            .LoadAsync();
    }
}