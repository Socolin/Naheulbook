using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IJobRepository : IRepository<JobEntity>
{
    Task<ICollection<JobEntity>> GetAllWithAllDataAsync();
}

public class JobRepository : Repository<JobEntity, NaheulbookDbContext>, IJobRepository
{
    public JobRepository(NaheulbookDbContext context)
        : base(context)
    {
    }

    public async Task<ICollection<JobEntity>> GetAllWithAllDataAsync()
    {
        return await Context.Jobs
            .Include(j => j.Bonuses)
            .Include(j => j.Requirements)
            .Include(j => j.Restrictions)
            .Include(j => j.Skills)
            .Include(j => j.Specialities).ThenInclude(s => s.Specials)
            .Include(j => j.Specialities).ThenInclude(s => s.Modifiers)
            .ToListAsync();
    }
}