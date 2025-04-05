using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface ILootRepository : IRepository<LootEntity>
{
    Task<List<LootEntity>> GetByGroupIdAsync(int groupId);
    Task<List<LootEntity>> GetLootsVisibleByCharactersOfGroupAsync(int groupId);
    Task<LootEntity?> GetWithAllDataAsync(int lootId);
}

public class LootRepository(NaheulbookDbContext context) : Repository<LootEntity, NaheulbookDbContext>(context), ILootRepository
{
    public Task<List<LootEntity>> GetByGroupIdAsync(int groupId)
    {
        return Context.Loots
            .IncludeItemDetails(l => l.Items)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .Where(l => l.GroupId == groupId)
            .ToListAsync();
    }

    public Task<List<LootEntity>> GetLootsVisibleByCharactersOfGroupAsync(int groupId)
    {
        return Context.Loots
            .IncludeItemDetails(l => l.Items)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .Where(l => l.IsVisibleForPlayer)
            .Where(l => l.GroupId == groupId)
            .ToListAsync();
    }

    public Task<LootEntity?> GetWithAllDataAsync(int lootId)
    {
        return Context.Loots
            .IncludeItemDetails(l => l.Items)
            .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
            .FirstOrDefaultAsync(l => l.Id == lootId);
    }
}