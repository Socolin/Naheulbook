#pragma warning disable 8619
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IEffectRepository : IRepository<EffectEntity>
{
    Task<ICollection<EffectTypeEntity>> GetCategoriesAsync();
    Task<ICollection<EffectEntity>> GetBySubCategoryWithModifiersAsync(long subCategoryId);
    Task<EffectEntity?> GetWithModifiersAsync(int effectId);
    Task<List<EffectEntity>> SearchByNameAsync(string partialName, int maxResult);
    Task<EffectEntity?> GetWithEffectWithModifiersAsync(int effectId);
}

public class EffectRepository(NaheulbookDbContext naheulbookDbContext) : Repository<EffectEntity, NaheulbookDbContext>(naheulbookDbContext), IEffectRepository
{
    public async Task<ICollection<EffectTypeEntity>> GetCategoriesAsync()
    {
        return await Context.EffectTypes
            .Include(e => e.SubCategories)
            .ToListAsync();
    }

    public async Task<ICollection<EffectEntity>> GetBySubCategoryWithModifiersAsync(long subCategoryId)
    {
        return await Context.Effects
            .Where(e => e.SubCategoryId == subCategoryId)
            .Include(e => e.Modifiers)
            .ToListAsync();
    }

    public Task<EffectEntity?> GetWithModifiersAsync(int effectId)
    {
        return Context.Effects
            .Include(e => e.Modifiers)
            .FirstAsync(e => e.Id == effectId);
    }

    public Task<List<EffectEntity>> SearchByNameAsync(string partialName, int maxResult)
    {
        return Context.Effects
            .Include(e => e.Modifiers)
            .Where(e => e.Name.ToUpper().Contains(partialName.ToUpper()))
            .Take(maxResult)
            .ToListAsync();
    }

    public Task<EffectEntity?> GetWithEffectWithModifiersAsync(int effectId)
    {
        return Context.Effects
            .Include(e => e.SubCategory)
            .Include(e => e.Modifiers)
            .FirstAsync(e => e.Id == effectId);
    }
}