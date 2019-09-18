using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Extensions;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterTemplateRepository : IRepository<MonsterTemplate>
    {
        Task<List<MonsterTemplate>> GetAllWithItemsFullDataWithLocationsAsync();
        Task<List<MonsterTemplate>> SearchByNameAsync(string filter, int maxResult);
        Task<MonsterTemplate> GetByIdWithItemsWithLocationsAsync(int monsterTemplateId);
        Task<MonsterTemplate> GetByIdWithItemsFullDataWithLocationsAsync(int monsterTemplateId);
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
                .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
                .ToListAsync();
        }

        public Task<List<MonsterTemplate>> SearchByNameAsync(string partialName, int maxResult)
        {
            return Context.MonsterTemplates
                .Include(x => x.Locations)
                .Include(m => m.Items)
                .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
                .Where(e => e.Name.ToUpper().Contains(partialName.ToUpper()))
                .Take(maxResult)
                .ToListAsync();
        }

        public Task<MonsterTemplate> GetByIdWithItemsWithLocationsAsync(int monsterTemplateId)
        {
            return Context.MonsterTemplates
                .Include(x => x.Locations)
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.Id == monsterTemplateId);
        }

        public Task<MonsterTemplate> GetByIdWithItemsFullDataWithLocationsAsync(int monsterTemplateId)
        {
            return Context.MonsterTemplates
                .Include(x => x.Locations)
                .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
                .SingleOrDefaultAsync(x => x.Id == monsterTemplateId);
        }
    }
}