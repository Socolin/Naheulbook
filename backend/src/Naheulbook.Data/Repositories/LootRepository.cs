using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Extensions;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ILootRepository : IRepository<LootEntity>
{
    Task<List<LootEntity>> GetByGroupIdAsync(int groupId);
    Task<List<LootEntity>> GetLootsVisibleByCharactersOfGroupAsync(int groupId);
    Task<LootEntity?> GetWithAllDataAsync(int lootId);
}

public class LootRepository : Repository<LootEntity, NaheulbookDbContext>, ILootRepository
{
    public LootRepository(NaheulbookDbContext context)
        : base(context)
    {
    }

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