using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<ICollection<Job>> GetAllWithAllDataAsync();
    }

    public class JobRepository : Repository<Job, NaheulbookDbContext>, IJobRepository
    {
        public JobRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public async Task<ICollection<Job>> GetAllWithAllDataAsync()
        {
            return await Context.Jobs
                .Include(j => j.Bonuses)
                .Include(j => j.Requirements)
                .Include(j => j.Restrictions)
                .Include(j => j.Skills)
                .Include(j => j.OriginBlacklist).ThenInclude(b => b.Origin)
                .Include(j => j.OriginWhitelist).ThenInclude(b => b.Origin)
                .Include(j => j.Specialities).ThenInclude(s => s.Specials)
                .Include(j => j.Specialities).ThenInclude(s => s.Modifiers)
                .ToListAsync();
        }
    }
}