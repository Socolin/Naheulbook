#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface IMonsterTemplateRepository : IRepository<MonsterTemplateEntity>
{
    Task<List<MonsterTemplateEntity>> GetAllWithItemsFullDataAsync(CancellationToken cancellationToken = default);
    Task<List<MonsterTemplateEntity>> SearchByNameAndSubCategoryAsync(string filter, int? monsterTypeId, int? monsterSubCategoryId, int maxResult, CancellationToken cancellationToken = default);
    Task<MonsterTemplateEntity?> GetByIdWithItemsAsync(int monsterTemplateId, CancellationToken cancellationToken = default);
    Task<MonsterTemplateEntity?> GetByIdWithItemsFullDataAsync(int monsterTemplateId, CancellationToken cancellationToken = default);
}

public class MonsterTemplateRepository(NaheulbookDbContext context) : Repository<MonsterTemplateEntity, NaheulbookDbContext>(context), IMonsterTemplateRepository
{
    public Task<List<MonsterTemplateEntity>> GetAllWithItemsFullDataAsync(CancellationToken cancellationToken = default)
    {
        return Context.MonsterTemplates
            .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
            .ToListAsync(cancellationToken);
    }

    public Task<List<MonsterTemplateEntity>> SearchByNameAndSubCategoryAsync(string partialName, int? monsterTypeId, int? monsterSubCategoryId, int maxResult, CancellationToken cancellationToken = default)
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
            .ToListAsync(cancellationToken);
    }

    public Task<MonsterTemplateEntity?> GetByIdWithItemsAsync(int monsterTemplateId, CancellationToken cancellationToken = default)
    {
        return Context.MonsterTemplates
            .Include(x => x.Items)
            .SingleOrDefaultAsync(x => x.Id == monsterTemplateId, cancellationToken);
    }

    public Task<MonsterTemplateEntity?> GetByIdWithItemsFullDataAsync(int monsterTemplateId, CancellationToken cancellationToken = default)
    {
        return Context.MonsterTemplates
            .IncludeChildWithItemTemplateDetails(x => x.Items, x => x.ItemTemplate)
            .SingleOrDefaultAsync(x => x.Id == monsterTemplateId, cancellationToken);
    }
}