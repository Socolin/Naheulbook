using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ITalentGroupRepository : IRepository<TalentGroupEntity>
{
    Task<List<TalentGroupEntity>> GetAllWithTalentsAsync(
        CancellationToken cancellationToken = default
    );
}

public class TalentGroupRepository(NaheulbookDbContext context)
    : Repository<TalentGroupEntity, NaheulbookDbContext>(context), ITalentGroupRepository
{
    public async Task<List<TalentGroupEntity>> GetAllWithTalentsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await Context.TalentGroups
            .Include(x => x.Talents.OrderBy(t => t.DisplayOrder))
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
