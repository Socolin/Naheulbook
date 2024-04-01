using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IItemTemplateSectionRepository : IRepository<ItemTemplateSectionEntity>
{
    Task<List<ItemTemplateSectionEntity>> GetAllWithCategoriesAsync();
}

public class ItemTemplateSectionRepository(NaheulbookDbContext context) : Repository<ItemTemplateSectionEntity, NaheulbookDbContext>(context), IItemTemplateSectionRepository
{
    public Task<List<ItemTemplateSectionEntity>> GetAllWithCategoriesAsync()
    {
        return Context.ItemTemplateSections
            .Include(s => s.SubCategories)
            .ToListAsync();
    }
}