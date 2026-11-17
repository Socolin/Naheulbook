#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface IMonsterRepository : IRepository<MonsterEntity>
{
    Task<List<MonsterEntity>> GetByGroupIdWithInventoryAsync(int groupId, CancellationToken cancellationToken = default);
    Task<List<MonsterEntity>> GetDeadMonstersByGroupIdAsync(int groupId, int startIndex, int count, CancellationToken cancellationToken = default);
    Task<List<MonsterEntity>> GetWithItemsByGroupAndByIdAsync(int groupId, IEnumerable<int> monsterIds, CancellationToken cancellationToken = default);
    Task<MonsterEntity?> GetWithGroupAsync(int monsterId, CancellationToken cancellationToken = default);
    Task<MonsterEntity?> GetWithGroupWithItemsAsync(int monsterId, CancellationToken cancellationToken = default);
}

public class MonsterRepository(NaheulbookDbContext context) : Repository<MonsterEntity, NaheulbookDbContext>(context), IMonsterRepository
{
    public Task<List<MonsterEntity>> GetByGroupIdWithInventoryAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return Context.Monsters
            .IncludeItemDetails(m => m.Items)
            .Where(g => g.GroupId == groupId && g.FightId == null && g.Dead == null)
            .ToListAsync(cancellationToken);
    }

    public Task<List<MonsterEntity>> GetDeadMonstersByGroupIdAsync(int groupId, int startIndex, int count, CancellationToken cancellationToken = default)
    {
        return Context.Monsters
            .Where(g => g.GroupId == groupId && g.Dead != null)
            .OrderByDescending(d => d.Dead)
            .Skip(startIndex)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public Task<List<MonsterEntity>> GetWithItemsByGroupAndByIdAsync(int groupId, IEnumerable<int> monsterIds, CancellationToken cancellationToken = default)
    {
        return Context.Monsters
            .Include(m => m.Items)
            .Where(m => m.GroupId == groupId && monsterIds.Contains(m.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<MonsterEntity?> GetWithGroupAsync(int monsterId, CancellationToken cancellationToken = default)
    {
        return Context.Monsters
            .Include(m => m.Group)
            .SingleOrDefaultAsync(m => m.Id == monsterId, cancellationToken);
    }

    public Task<MonsterEntity?> GetWithGroupWithItemsAsync(int monsterId, CancellationToken cancellationToken = default)
    {
        return Context.Monsters
            .Include(m => m.Group)
            .Include(m => m.Items)
            .ThenInclude(i => i.ItemTemplate)
            .ThenInclude(i => i.UnSkills)
            .Include(m => m.Items)
            .ThenInclude(i => i.ItemTemplate)
            .ThenInclude(i => i.Skills)
            .Include(m => m.Items)
            .ThenInclude(i => i.ItemTemplate)
            .ThenInclude(i => i.Modifiers)
            .Include(m => m.Items)
            .ThenInclude(i => i.ItemTemplate)
            .ThenInclude(i => i.SkillModifiers)
            .Include(m => m.Items)
            .ThenInclude(i => i.ItemTemplate)
            .ThenInclude(i => i.Requirements)
            .Include(m => m.Items)
            .ThenInclude(i => i.ItemTemplate)
            .ThenInclude(i => i.Slots)
            .ThenInclude(i => i.Slot)
            .Include(m => m.Items)
            .ThenInclude(i => i.ItemTemplate)
            .ThenInclude(i => i.Modifiers)
            .SingleOrDefaultAsync(m => m.Id == monsterId, cancellationToken);
    }
}