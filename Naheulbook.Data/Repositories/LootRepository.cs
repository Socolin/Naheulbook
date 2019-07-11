using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Extensions;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ILootRepository : IRepository<Loot>
    {
        Task<List<Loot>> GetByGroupIdAsync(int groupId);
        Task<List<Loot>> GetLootsVisibleByCharactersOfGroupAsync(int groupId);
        Task<Loot> GetWithAllDataAsync(int lootId);
    }

    public class LootRepository : Repository<Loot, NaheulbookDbContext>, ILootRepository
    {
        public LootRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<Loot>> GetByGroupIdAsync(int groupId)
        {
            return Context.Loots
                .IncludeItemDetails(l => l.Items)
                .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
                .Where(l => l.GroupId == groupId)
                .ToListAsync();
        }

        public Task<List<Loot>> GetLootsVisibleByCharactersOfGroupAsync(int groupId)
        {
            return Context.Loots
                .IncludeItemDetails(l => l.Items)
                .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
                .Where(l => l.IsVisibleForPlayer)
                .Where(l => l.GroupId == groupId)
                .ToListAsync();
        }

        public Task<Loot> GetWithAllDataAsync(int lootId)
        {
            return Context.Loots
                .IncludeItemDetails(l => l.Items)
                .IncludeChildWithItemDetails(m => m.Monsters, l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == lootId);
        }
    }

}