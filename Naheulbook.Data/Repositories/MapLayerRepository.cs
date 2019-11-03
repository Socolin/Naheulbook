using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMapLayerRepository : IRepository<MapLayer>
    {
        Task LoadMarkersForResponseAsync(MapLayer mapLayer);
    }

    public class MapLayerRepository : Repository<MapLayer, NaheulbookDbContext>, IMapLayerRepository
    {
        public MapLayerRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task LoadMarkersForResponseAsync(MapLayer mapLayer)
        {
            return Context.Entry(mapLayer)
                .Collection(x => x.Markers)
                .Query()
                .Include(x => x.Links)
                .ThenInclude(x => x.TargetMap)
                .LoadAsync();
        }
    }
}