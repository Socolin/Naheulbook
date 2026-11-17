using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface ILootRepository : IRepository<LootEntity>
{
    Task<List<LootEntity>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default);
    Task<List<LootEntity>> GetLootsVisibleByCharactersOfGroupAsync(int groupId, CancellationToken cancellationToken = default);
    Task<LootEntity?> GetWithAllDataAsync(int lootId, CancellationToken cancellationToken = default);
}

public class LootRepository(NaheulbookDbContext context) : Repository<LootEntity, NaheulbookDbContext>(context), ILootRepository
{
    public Task<List<LootEntity>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return Context.Loots
            .IncludeItemDetails(l => l.Items)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .Where(l => l.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<LootEntity>> GetLootsVisibleByCharactersOfGroupAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return Context.Loots
            .IncludeItemDetails(l => l.Items)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .Where(l => l.IsVisibleForPlayer)
            .Where(l => l.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }

    public Task<LootEntity?> GetWithAllDataAsync(int lootId, CancellationToken cancellationToken = default)
    {
        return Context.Loots
            .IncludeItemDetails(l => l.Items)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == lootId, cancellationToken);
    }
}