using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ISpecialAttackGroupRepository : IRepository<SpecialAttackGroupEntity>
{
    Task<List<SpecialAttackGroupEntity>> GetAllWithSpecialAttacksAsync(
        CancellationToken cancellationToken = default
    );
}

public class SpecialAttackGroupRepository(NaheulbookDbContext context)
    : Repository<SpecialAttackGroupEntity, NaheulbookDbContext>(context), ISpecialAttackGroupRepository
{
    public async Task<List<SpecialAttackGroupEntity>> GetAllWithSpecialAttacksAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Context.SpecialAttackGroups
            .Include(x => x.SpecialAttacks.OrderBy(a => a.DisplayOrder))
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
