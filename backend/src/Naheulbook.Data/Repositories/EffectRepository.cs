#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IEffectRepository : IRepository<EffectEntity>
{
    Task<ICollection<EffectTypeEntity>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<ICollection<EffectEntity>> GetBySubCategoryWithModifiersAsync(long subCategoryId, CancellationToken cancellationToken = default);
    Task<EffectEntity?> GetWithModifiersAsync(int effectId, CancellationToken cancellationToken = default);
    Task<List<EffectEntity>> SearchByNameAsync(string partialName, int maxResult, CancellationToken cancellationToken = default);
    Task<EffectEntity?> GetWithEffectWithModifiersAsync(int effectId, CancellationToken cancellationToken = default);
}

public class EffectRepository(NaheulbookDbContext naheulbookDbContext) : Repository<EffectEntity, NaheulbookDbContext>(naheulbookDbContext), IEffectRepository
{
    public async Task<ICollection<EffectTypeEntity>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.EffectTypes
            .Include(e => e.SubCategories)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<EffectEntity>> GetBySubCategoryWithModifiersAsync(long subCategoryId, CancellationToken cancellationToken = default)
    {
        return await Context.Effects
            .Where(e => e.SubCategoryId == subCategoryId)
            .Include(e => e.Modifiers)
            .ToListAsync(cancellationToken);
    }

    public Task<EffectEntity?> GetWithModifiersAsync(int effectId, CancellationToken cancellationToken = default)
    {
        return Context.Effects
            .Include(e => e.Modifiers)
            .FirstAsync(e => e.Id == effectId, cancellationToken);
    }

    public Task<List<EffectEntity>> SearchByNameAsync(string partialName, int maxResult, CancellationToken cancellationToken = default)
    {
        return Context.Effects
            .Include(e => e.Modifiers)
            .Where(e => e.Name.ToUpper().Contains(partialName.ToUpper()))
            .Take(maxResult)
            .ToListAsync(cancellationToken);
    }

    public Task<EffectEntity?> GetWithEffectWithModifiersAsync(int effectId, CancellationToken cancellationToken = default)
    {
        return Context.Effects
            .Include(e => e.SubCategory)
            .Include(e => e.Modifiers)
            .FirstAsync(e => e.Id == effectId, cancellationToken);
    }
}