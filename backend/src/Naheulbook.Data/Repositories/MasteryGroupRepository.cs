using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMasteryGroupRepository : IRepository<MasteryGroupEntity>
{
    Task<List<MasteryGroupEntity>> GetAllWithMasteriesAsync(
        CancellationToken cancellationToken = default
    );
}

public class MasteryGroupRepository(NaheulbookDbContext context)
    : Repository<MasteryGroupEntity, NaheulbookDbContext>(context), IMasteryGroupRepository
{
    public async Task<List<MasteryGroupEntity>> GetAllWithMasteriesAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Context.MasteryGroups
            .Include(x => x.Masteries.OrderBy(m => m.DisplayOrder))
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
