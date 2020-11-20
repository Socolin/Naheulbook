#pragma warning disable 8619
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
        Task<List<MonsterTemplate>> GetAllWithItemsFullDataAsync();
        Task<List<MonsterTemplate>> SearchByNameAndSubCategoryAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId, int maxResult);
        Task<MonsterTemplate?> GetByIdWithItemsAsync(int monsterTemplateId);
        Task<MonsterTemplate?> GetByIdWithItemsFullDataAsync(int monsterTemplateId);
    }

    public class MonsterTemplateRepository : Repository<MonsterTemplate, NaheulbookDbContext>, IMonsterTemplateRepository
    {
        public MonsterTemplateRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<MonsterTemplate>> GetAllWithItemsFullDataAsync()
        {
            return Context.MonsterTemplates
                .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
                .ToListAsync();
        }

        public Task<List<MonsterTemplate>> SearchByNameAndSubCategoryAsync(string partialName, int? monsterTypeId, int? monsterSubCategoryId, int maxResult)
        {
            var query = Context.MonsterTemplates
                .Include(m => m.Items)
                .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
                .Where(e => e.Name.ToUpper().Contains(partialName.ToUpper()));

            if (monsterSubCategoryId.HasValue)
            {
                query = query.Where(e => e.SubCategoryId == monsterSubCategoryId.Value);
            }
            else if (monsterTypeId.HasValue)
            {
                query = query.Where(e => e.SubCategory.TypeId == monsterTypeId.Value);
            }

            return query
                .Take(maxResult)
                .ToListAsync();
        }

        public Task<MonsterTemplate?> GetByIdWithItemsAsync(int monsterTemplateId)
        {
            return Context.MonsterTemplates
                .Include(x => x.Items)
                .SingleOrDefaultAsync(x => x.Id == monsterTemplateId);
        }

        public Task<MonsterTemplate?> GetByIdWithItemsFullDataAsync(int monsterTemplateId)
        {
            return Context.MonsterTemplates
                .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
                .SingleOrDefaultAsync(x => x.Id == monsterTemplateId);
        }
    }
}