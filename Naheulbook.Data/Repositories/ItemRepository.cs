using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<Item> GetWithAllDataAsync(int itemId);
        Task<Item> GetWithOwnerAsync(int itemId);
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
                .Include(i => i.ItemTemplate)
                .ThenInclude(i => i.UnSkills)
                .Include(i => i.ItemTemplate)
                .ThenInclude(i => i.Skills)
                .Include(i => i.ItemTemplate)
                .ThenInclude(i => i.Modifiers)
                .Include(i => i.ItemTemplate)
                .ThenInclude(i => i.SkillModifiers)
                .Include(i => i.ItemTemplate)
                .ThenInclude(i => i.Requirements)
                .Include(i => i.ItemTemplate)
                .ThenInclude(i => i.Slots)
                .ThenInclude(i => i.Slot)
                .Include(i => i.ItemTemplate)
                .ThenInclude(i => i.Modifiers)
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
    }
}