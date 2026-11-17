using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

#pragma warning disable 8619

namespace Naheulbook.Data.Repositories;

public interface IItemRepository : IRepository<ItemEntity>
{
    Task<ItemEntity?> GetWithAllDataAsync(int itemId, CancellationToken cancellationToken = default);
    Task<List<ItemEntity>> GetWithAllDataByIdsAsync(IEnumerable<int> itemIds, CancellationToken cancellationToken = default);
    Task<ItemEntity?> GetWithOwnerAsync(int itemId, CancellationToken cancellationToken = default);
    Task<ItemEntity?> GetWithOwnerWitGroupCharactersAsync(int itemId, CancellationToken cancellationToken = default);
    Task<ItemEntity?> GetWithAllDataWithCharacterAsync(int itemId, CancellationToken cancellationToken = default);
}

public class ItemRepository(NaheulbookDbContext context) : Repository<ItemEntity, NaheulbookDbContext>(context), IItemRepository
{
    public Task<ItemEntity?> GetWithAllDataAsync(int itemId, CancellationToken cancellationToken = default)
    {
        return Context.Items
            .IncludeItemTemplateDetails(i => i.ItemTemplate)
            .SingleAsync(x => x.Id == itemId, cancellationToken);
    }

    public Task<List<ItemEntity>> GetWithAllDataByIdsAsync(IEnumerable<int> itemIds, CancellationToken cancellationToken = default)
    {
        return Context.Items
            .IncludeItemTemplateDetails(i => i.ItemTemplate)
            .Where(x => itemIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<ItemEntity?> GetWithOwnerAsync(int itemId, CancellationToken cancellationToken = default)
    {
        return Context.Items
            .Include(i => i.Character!)
            .ThenInclude(c => c.Group)
            .Include(i => i.Monster!)
            .ThenInclude(m => m.Group)
            .Include(i => i.Loot!)
            .ThenInclude(l => l.Group)
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);
    }

    public Task<ItemEntity?> GetWithOwnerWitGroupCharactersAsync(int itemId, CancellationToken cancellationToken = default)
    {
        return Context.Items
            .Include(i => i.Character!)
            .ThenInclude(c => c.Group!)
            .ThenInclude(m => m.Characters)
            .Include(i => i.Monster!)
            .ThenInclude(m => m.Group)
            .ThenInclude(m => m.Characters)
            .Include(i => i.Loot!)
            .ThenInclude(l => l.Group)
            .ThenInclude(m => m.Characters)
            .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);
    }

    public Task<ItemEntity?> GetWithAllDataWithCharacterAsync(int itemId, CancellationToken cancellationToken = default)
    {
        return Context.Items
            .IncludeItemTemplateDetails(i => i.ItemTemplate)
            .Include(i => i.Character)
            .SingleAsync(x => x.Id == itemId, cancellationToken);
    }
}