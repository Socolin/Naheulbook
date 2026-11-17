using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface IFightRepository : IRepository<FightEntity>
{
    Task<List<FightEntity>> GetByGroupIdWithMonstersAsync(int groupId, CancellationToken cancellationToken = default);
    Task<FightEntity?> GetWithMonstersAsync(int fightId, CancellationToken cancellationToken = default);
}

public class FightRepository(NaheulbookDbContext context) : Repository<FightEntity, NaheulbookDbContext>(context), IFightRepository
{
    public Task<List<FightEntity>> GetByGroupIdWithMonstersAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return Context.Fights
            .Where(f => f.GroupId == groupId)
            .Include(f => f.Monsters)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .ToListAsync(cancellationToken);
    }

    public Task<FightEntity?> GetWithMonstersAsync(int fightId, CancellationToken cancellationToken = default)
    {
        return Context.Fights
            .Where(f => f.Id == fightId)
            .Include(f => f.Monsters)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .SingleOrDefaultAsync(cancellationToken);
    }
}