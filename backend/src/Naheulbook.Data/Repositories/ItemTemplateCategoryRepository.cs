#pragma warning disable 8619
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IItemTemplateSubCategoryRepository : IRepository<ItemTemplateSubCategoryEntity>
{
    Task<ItemTemplateSubCategoryEntity?> GetWithItemTemplatesByTechNameAsync(string techName);
    Task<ItemTemplateSubCategoryEntity?> GetByTechNameAsync(string techName);
}

public class ItemTemplateSubCategoryRepository(NaheulbookDbContext context) : Repository<ItemTemplateSubCategoryEntity, NaheulbookDbContext>(context), IItemTemplateSubCategoryRepository
{
    public Task<ItemTemplateSubCategoryEntity?> GetWithItemTemplatesByTechNameAsync(string techName)
    {
        return Context.ItemTemplateSubCategories
            .Include(c => c.ItemTemplates)
            .SingleOrDefaultAsync(c => c.TechName == techName);
    }

    public Task<ItemTemplateSubCategoryEntity?> GetByTechNameAsync(string techName)
    {
        return Context.ItemTemplateSubCategories
            .SingleOrDefaultAsync(c => c.TechName == techName);
    }
}