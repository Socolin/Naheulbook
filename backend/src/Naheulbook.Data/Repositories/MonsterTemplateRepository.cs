#pragma warning disable 8619
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface IMonsterTemplateRepository : IRepository<MonsterTemplateEntity>
{
    Task<List<MonsterTemplateEntity>> GetAllWithItemsFullDataAsync();
    Task<List<MonsterTemplateEntity>> SearchByNameAndSubCategoryAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId, int maxResult);
    Task<MonsterTemplateEntity?> GetByIdWithItemsAsync(int monsterTemplateId);
    Task<MonsterTemplateEntity?> GetByIdWithItemsFullDataAsync(int monsterTemplateId);
}

public class MonsterTemplateRepository(NaheulbookDbContext context) : Repository<MonsterTemplateEntity, NaheulbookDbContext>(context), IMonsterTemplateRepository
{
    public Task<List<MonsterTemplateEntity>> GetAllWithItemsFullDataAsync()
    {
        return Context.MonsterTemplates
            .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
            .ToListAsync();
    }

    public Task<List<MonsterTemplateEntity>> SearchByNameAndSubCategoryAsync(string partialName, int? monsterTypeId, int? monsterSubCategoryId, int maxResult)
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

    public Task<MonsterTemplateEntity?> GetByIdWithItemsAsync(int monsterTemplateId)
    {
        return Context.MonsterTemplates
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == monsterTemplateId);
    }

    public Task<MonsterTemplateEntity?> GetByIdWithItemsFullDataAsync(int monsterTemplateId)
    {
        return Context.MonsterTemplates
            .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
            .SingleOrDefaultAsync(x => x.Id == monsterTemplateId);
    }
}