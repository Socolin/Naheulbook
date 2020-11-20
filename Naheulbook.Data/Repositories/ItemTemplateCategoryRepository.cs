#pragma warning disable 8619
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTemplateSubCategoryRepository : IRepository<ItemTemplateSubCategory>
    {
        Task<ItemTemplateSubCategory?> GetWithItemTemplatesByTechNameAsync(string techName);
        Task<ItemTemplateSubCategory?> GetByTechNameAsync(string techName);
    }

    public class ItemTemplateSubCategoryRepository : Repository<ItemTemplateSubCategory, NaheulbookDbContext>, IItemTemplateSubCategoryRepository
    {
        public ItemTemplateSubCategoryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<ItemTemplateSubCategory?> GetWithItemTemplatesByTechNameAsync(string techName)
        {
            return Context.ItemTemplateSubCategories
                .Include(c => c.ItemTemplates)
                .SingleOrDefaultAsync(c => c.TechName == techName);
        }

        public Task<ItemTemplateSubCategory?> GetByTechNameAsync(string techName)
        {
            return Context.ItemTemplateSubCategories
                .SingleOrDefaultAsync(c => c.TechName == techName);
        }
    }
}