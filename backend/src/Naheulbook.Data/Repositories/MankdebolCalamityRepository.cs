using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMankdebolCalamityRepository : IRepository<MankdebolCalamityEntity>
{
    Task<List<MankdebolCalamityEntity>> GetAllOrderedByDiceRollAsync(
        CancellationToken cancellationToken = default
    );
}

public class MankdebolCalamityRepository(NaheulbookDbContext context)
    : Repository<MankdebolCalamityEntity, NaheulbookDbContext>(context), IMankdebolCalamityRepository
{
    public async Task<List<MankdebolCalamityEntity>> GetAllOrderedByDiceRollAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Context.MankdebolCalamities
            .OrderBy(x => x.DiceRoll)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
