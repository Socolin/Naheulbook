using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterTemplateRepository : IRepository<MonsterTemplate>
    {
        Task<List<MonsterTemplate>> GetAllWithItemsFullDataWithLocationsAsync();
        Task<List<MonsterTemplate>> SearchByNameAsync(string filter, int maxResult);
    }

    public class MonsterTemplateRepository : Repository<MonsterTemplate, NaheulbookDbContext>, IMonsterTemplateRepository
    {
        public MonsterTemplateRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<MonsterTemplate>> GetAllWithItemsFullDataWithLocationsAsync()
        {
            return Context.MonsterTemplates
                .Include(x => x.Locations)
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
                .ToListAsync();
        }

        public Task<List<MonsterTemplate>> SearchByNameAsync(string partialName, int maxResult)
        {
            return Context.MonsterTemplates
                .Include(x => x.Locations)
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
                .Where(e => e.Name.ToUpper().Contains(partialName.ToUpper()))
                .Take(maxResult)
                .ToListAsync();
        }
    }
}