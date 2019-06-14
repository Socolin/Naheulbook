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
    }

    public class LocationRepository : Repository<Location, NaheulbookDbContext>, ILocationRepository
    {
        public LocationRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<Location>> GetByIdsAsync(IEnumerable<int> locationIds)
        {
            return Context.Location
                .Where(x => locationIds.Contains(x.Id))
                .ToListAsync();
        }

        public Task<Location> GetNewGroupDefaultLocationAsync()
        {
            return Context.Location.SingleAsync(x => x.Id == 1);
        }
    }
}