using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IOriginRepository : IRepository<OriginEntity>
{
    Task<ICollection<OriginEntity>> GetAllWithAllDataAsync();
    Task<OriginEntity?> GetWithAllDataAsync(Guid originId);
}

public class OriginRepository : Repository<OriginEntity, NaheulbookDbContext>, IOriginRepository
{
    public OriginRepository(NaheulbookDbContext context) : base(context)
    {
    }

    public async Task<ICollection<OriginEntity>> GetAllWithAllDataAsync()
    {
        return await Context.Origins
            .Include(o => o.Requirements)
            .Include(o => o.Information)
            .Include(o => o.Restrictions)
            .Include(o => o.Bonuses)
            .Include(o => o.Skills)
            .ToListAsync();
    }

    public Task<OriginEntity?> GetWithAllDataAsync(Guid originId)
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