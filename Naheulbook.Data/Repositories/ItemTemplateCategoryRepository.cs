using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTemplateCategoryRepository : IRepository<ItemTemplateCategory>
    {
        Task<ItemTemplateCategory> GetWithItemTemplatesByTechNameAsync(string techName);
    }

    public class ItemTemplateCategoryRepository : Repository<ItemTemplateCategory, NaheulbookDbContext>, IItemTemplateCategoryRepository
    {
        public ItemTemplateCategoryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<ItemTemplateCategory> GetWithItemTemplatesByTechNameAsync(string techName)
        {
            return Context.ItemTemplateCategories
                .Include(c => c.ItemTemplates)
                .SingleOrDefaultAsync(c => c.TechName == techName);
        }
    }
}