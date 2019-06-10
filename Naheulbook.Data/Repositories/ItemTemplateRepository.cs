using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTemplateRepository : IRepository<ItemTemplate>
    {
        Task<ItemTemplate> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(int id);
        Task<List<ItemTemplate>> GetByIdsAsync(IEnumerable<int> ids);
    }

    public class ItemTemplateRepository : Repository<ItemTemplate, NaheulbookDbContext>, IItemTemplateRepository
    {
        public ItemTemplateRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<ItemTemplate> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(int id)
        {
            return Context.ItemTemplates
                .Include(x => x.Requirements)
                .Include(x => x.Modifiers)
                .Include(x => x.Slots)
                .ThenInclude(x => x.Slot)
                .Include(x => x.Skills)
                .Include(x => x.UnSkills)
                .Include(x => x.SkillModifiers)
                .Where(x => x.Id == id)
                .SingleAsync();
        }

        public Task<List<ItemTemplate>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return Context.ItemTemplates
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }
    }
}