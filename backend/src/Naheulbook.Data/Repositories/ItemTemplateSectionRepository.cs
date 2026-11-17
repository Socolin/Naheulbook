using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IItemTemplateSectionRepository : IRepository<ItemTemplateSectionEntity>
{
    Task<List<ItemTemplateSectionEntity>> GetAllWithCategoriesAsync(CancellationToken cancellationToken = default);
}

public class ItemTemplateSectionRepository(NaheulbookDbContext context) : Repository<ItemTemplateSectionEntity, NaheulbookDbContext>(context), IItemTemplateSectionRepository
{
    public Task<List<ItemTemplateSectionEntity>> GetAllWithCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplateSections
            .Include(s => s.SubCategories)
            .ToListAsync(cancellationToken);
    }
}