using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IOriginRepository : IRepository<OriginEntity>
{
    Task<ICollection<OriginEntity>> GetAllWithAllDataAsync();
    Task<OriginEntity?> GetWithAllDataAsync(Guid originId);
}

public class OriginRepository(NaheulbookDbContext context) : Repository<OriginEntity, NaheulbookDbContext>(context), IOriginRepository
{
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