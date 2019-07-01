using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<List<Location>> GetByIdsAsync(IEnumerable<int> locationIds);
        Task<Location> GetNewGroupDefaultLocationAsync();
        Task<List<LocationMap>> GetLocationMapsByLocationIdAsync(int locationId);
        Task<List<Location>> SearchByNameAsync(string filter, int maxCount);
    }

    public class LocationRepository : Repository<Location, NaheulbookDbContext>, ILocationRepository
    {
        public LocationRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<Location>> GetByIdsAsync(IEnumerable<int> locationIds)
        {
            return Context.Locations
                .Where(x => locationIds.Contains(x.Id))
                .ToListAsync();
        }

        public Task<Location> GetNewGroupDefaultLocationAsync()
        {
            return Context.Locations.SingleAsync(x => x.Id == 1);
        }

        public Task<List<LocationMap>> GetLocationMapsByLocationIdAsync(int locationId)
        {
            return Context.LocationMaps
                .Where(x => x.LocationId == locationId)
                .ToListAsync();
        }

        public Task<List<Location>> SearchByNameAsync(string partialName, int maxResult)
        {
            return Context.Locations
                .Where(e => e.Name.ToUpper().Contains(partialName.ToUpper()))
                .Take(maxResult)
                .ToListAsync();
        }
    }
}