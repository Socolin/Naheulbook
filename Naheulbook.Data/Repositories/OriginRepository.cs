using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IOriginRepository : IRepository<Origin>
    {
        ValueTask<Origin> GetAsync(Guid id);
        Task<ICollection<Origin>> GetAllWithAllDataAsync();
        Task<Origin> GetWithAllDataAsync(Guid originId);
    }

    public class OriginRepository : Repository<Origin, NaheulbookDbContext>, IOriginRepository
    {
        public OriginRepository(NaheulbookDbContext context) : base(context)
        {
        }

        public ValueTask<Origin> GetAsync(Guid id)
        {
            return Context.Origins.FindAsync(id);
        }

        public async Task<ICollection<Origin>> GetAllWithAllDataAsync()
        {
            return await Context.Origins
                .Include(o => o.Requirements)
                .Include(o => o.Information)
                .Include(o => o.Restrictions)
                .Include(o => o.Bonuses)
                .Include(o => o.Skills)
                .ToListAsync();
        }

        public Task<Origin> GetWithAllDataAsync(Guid originId)
        {
            return Context.Origins
                .Include(o => o.Requirements)
                .Include(o => o.Information)
                .Include(o => o.Restrictions)
                .Include(o => o.Bonuses)
                .Include(o => o.Skills)
                .SingleOrDefaultAsync(o => o.Id == originId);
        }
    }
}