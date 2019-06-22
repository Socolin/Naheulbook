using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ILootRepository : IRepository<Loot>
    {
        Task<List<Loot>> GetLootsVisibleByCharactersOfGroupAsync(int groupId);
    }

    public class LootRepository : Repository<Loot, NaheulbookDbContext>, ILootRepository
    {
        public LootRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<Loot>> GetLootsVisibleByCharactersOfGroupAsync(int groupId)
        {
            return Context.Loots
                .Include(l => l.Monsters)
                .ThenInclude(m => m.Items)
                .ThenInclude(i => i.ItemTemplate)
                .Include(l => l.Items)
                .ThenInclude(i => i.ItemTemplate)
                .Where(l => l.IsVisibleForPlayer)
                .Where(l => l.GroupId == groupId)
                .ToListAsync();
        }
    }
}