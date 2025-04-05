using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface IFightRepository : IRepository<FightEntity>
{
    Task<List<FightEntity>> GetByGroupIdWithMonstersAsync(int groupId);
    Task<FightEntity?> GetWithMonstersAsync(int fightId);
}

public class FightRepository(NaheulbookDbContext context) : Repository<FightEntity, NaheulbookDbContext>(context), IFightRepository
{
    public Task<List<FightEntity>> GetByGroupIdWithMonstersAsync(int groupId)
    {
        return Context.Fights
            .Where(f => f.GroupId == groupId)
            .Include(f => f.Monsters)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .ToListAsync();
    }

    public Task<FightEntity?> GetWithMonstersAsync(int fightId)
    {
        return Context.Fights
            .Where(f => f.Id == fightId)
            .Include(f => f.Monsters)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .SingleOrDefaultAsync();
    }
}