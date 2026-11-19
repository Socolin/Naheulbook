using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IHandicapRepository : IRepository<HandicapEntity>
{
    Task<List<HandicapEntity>> GetAllOrderedByDiceRollAsync(
        CancellationToken cancellationToken = default
    );
}

public class HandicapRepository(NaheulbookDbContext context)
    : Repository<HandicapEntity, NaheulbookDbContext>(context), IHandicapRepository
{
    public async Task<List<HandicapEntity>> GetAllOrderedByDiceRollAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Context.Handicaps
            .OrderBy(x => x.DiceRoll)
            .ToListAsync(cancellationToken);
    }
}
