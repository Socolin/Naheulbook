using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IEffectSubCategoryRepository : IRepository<EffectSubCategory>
    {
    }

    public class EffectSubCategoryRepository : Repository<EffectSubCategory, NaheulbookDbContext>, IEffectSubCategoryRepository
    {
        public EffectSubCategoryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}