using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IOriginRepository : IRepository<OriginEntity>
{
    Task<ICollection<OriginEntity>> GetAllWithAllDataAsync(CancellationToken cancellationToken = default);
    Task<OriginEntity?> GetWithAllDataAsync(Guid originId, CancellationToken cancellationToken = default);
}

public class OriginRepository(NaheulbookDbContext context) : Repository<OriginEntity, NaheulbookDbContext>(context), IOriginRepository
{
    public async Task<ICollection<OriginEntity>> GetAllWithAllDataAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Origins
            .Include(o => o.Requirements)
            .Include(o => o.Information)
            .Include(o => o.Restrictions)
            .Include(o => o.Bonuses)
            .Include(o => o.Skills)
            .ToListAsync(cancellationToken);
    }

    public Task<OriginEntity?> GetWithAllDataAsync(Guid originId, CancellationToken cancellationToken = default)
    {
        return Context.Origins
            .Include(o => o.Requirements)
            .Include(o => o.Information)
            .Include(o => o.Restrictions)
            .Include(o => o.Bonuses)
            .Include(o => o.Skills)
            .SingleOrDefaultAsync(o => o.Id == originId, cancellationToken);
    }
}