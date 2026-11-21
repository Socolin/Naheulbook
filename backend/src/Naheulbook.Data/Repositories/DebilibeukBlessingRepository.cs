using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IDebilibeukBlessingRepository : IRepository<DebilibeukBlessingEntity>
{
    Task<List<DebilibeukBlessingEntity>> GetAllOrderedByDiceRollAsync(
        CancellationToken cancellationToken = default
    );
}

public class DebilibeukBlessingRepository(NaheulbookDbContext context)
    : Repository<DebilibeukBlessingEntity, NaheulbookDbContext>(context), IDebilibeukBlessingRepository
{
    public async Task<List<DebilibeukBlessingEntity>> GetAllOrderedByDiceRollAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Context.DebilibeukBlessings
            .OrderBy(x => x.DiceRoll)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
