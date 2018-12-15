using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTemplateCategoryRepository : IRepository<ItemTemplateCategory>
    {
    }

    public class ItemTemplateCategoryRepository : Repository<ItemTemplateCategory, NaheulbookDbContext>, IItemTemplateCategoryRepository
    {
        public ItemTemplateCategoryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}