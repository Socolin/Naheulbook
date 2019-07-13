using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Extensions;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<Item> GetWithAllDataAsync(int itemId);
        Task<Item> GetWithOwnerAsync(int itemId);
        Task<Item> GetWithOwnerWitGroupCharactersAsync(int itemId);
        Task<Item> GetWithAllDataWithCharacterAsync(int itemId);
    }

    public class ItemRepository : Repository<Item, NaheulbookDbContext>, IItemRepository
    {
        public ItemRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<Item> GetWithAllDataAsync(int itemId)
        {
            return Context.Items
                .IncludeItemTemplateDetails(i => i.ItemTemplate)
                .SingleAsync(x => x.Id == itemId);
        }

        public Task<Item> GetWithOwnerAsync(int itemId)
        {
            return Context.Items
                .Include(i => i.Character)
                .ThenInclude(c => c.Group)
                .Include(i => i.Monster)
                .ThenInclude(m => m.Group)
                .Include(i => i.Loot)
                .ThenInclude(l => l.Group)
                .FirstOrDefaultAsync(i => i.Id == itemId);
        }

        public Task<Item> GetWithOwnerWitGroupCharactersAsync(int itemId)
        {
            return Context.Items
                .Include(i => i.Character)
                .ThenInclude(c => c.Group)
                .ThenInclude(m => m.Characters)
                .Include(i => i.Monster)
                .ThenInclude(m => m.Group)
                .ThenInclude(m => m.Characters)
                .Include(i => i.Loot)
                .ThenInclude(l => l.Group)
                .ThenInclude(m => m.Characters)
                .FirstOrDefaultAsync(i => i.Id == itemId);
        }

        public Task<Item> GetWithAllDataWithCharacterAsync(int itemId)
        {
            return Context.Items
                .IncludeItemTemplateDetails(i => i.ItemTemplate)
                .Include(i => i.Character)
                .SingleAsync(x => x.Id == itemId);
        }
    }
}