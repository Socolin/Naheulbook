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
        Task<List<ItemTemplate>> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(int sectionId);
        Task<List<ItemTemplate>> GetByIdsAsync(IEnumerable<int> ids);
        Task<List<ItemTemplate>> GetItemByCleanNameAsync(string name, int maxResultCount);
        Task<List<ItemTemplate>> GetItemByPartialCleanNameAsync(string name, int maxResultCount, IEnumerable<int> excludedIds);
        Task<List<ItemTemplate>> GetItemByPartialCleanNameWithoutSeparatorAsync(string name, int maxResultCount, IEnumerable<int> excludedIds);
        Task<ItemTemplate> GetPurseItemTemplateBasedOnMoneyAsync(int money);
        Task<ItemTemplate> GetGoldCoinItemTemplate();
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

        public Task<List<ItemTemplate>> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(int sectionId)
        {
            return Context.ItemTemplates
                .Include(x => x.Requirements)
                .Include(x => x.Modifiers)
                .Include(x => x.Slots)
                .ThenInclude(x => x.Slot)
                .Include(x => x.Skills)
                .Include(x => x.UnSkills)
                .Include(x => x.SkillModifiers)
                .Where(x => x.Category.SectionId == sectionId)
                .ToListAsync();
        }

        public Task<List<ItemTemplate>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return Context.ItemTemplates
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public Task<List<ItemTemplate>> GetItemByCleanNameAsync(string name, int maxResultCount)
        {
            return Context.ItemTemplates
                .Where(x => x.CleanName.ToUpper() == name)
                .Take(maxResultCount)
                .ToListAsync();
        }

        public Task<List<ItemTemplate>> GetItemByPartialCleanNameAsync(string name, int maxResultCount, IEnumerable<int> excludedIds)
        {
            return Context.ItemTemplates
                .Where(x => x.CleanName.ToUpper().Contains(name))
                .Take(maxResultCount)
                .Where(i => !excludedIds.Contains(i.Id))
                .ToListAsync();
        }

        public Task<List<ItemTemplate>> GetItemByPartialCleanNameWithoutSeparatorAsync(string name, int maxResultCount, IEnumerable<int> excludedIds)
        {
            return Context.ItemTemplates
                .Where(x => x.CleanName
                    .Replace("'", "")
                    .Replace("-", "")
                    .Replace(" ", "")
                    .ToUpper()
                    .Contains(name)
                )
                .Where(i => !excludedIds.Contains(i.Id))
                .Take(maxResultCount)
                .ToListAsync();
        }

        public Task<ItemTemplate> GetPurseItemTemplateBasedOnMoneyAsync(int money)
        {
            string techName;
            if (money <= 50)
                techName = "SMALL_PURSE";
            else if (money <= 100)
                techName = "MEDIUM_PURSE";
            else
                techName = "BIG_PURSE";

            return Context.ItemTemplates
                .Where(x => x.TechName == techName)
                .FirstAsync();
        }

        public Task<ItemTemplate> GetGoldCoinItemTemplate()
        {
            return Context.ItemTemplates
                .Where(x => x.TechName == "MONEY")
                .FirstAsync();
        }
    }
}