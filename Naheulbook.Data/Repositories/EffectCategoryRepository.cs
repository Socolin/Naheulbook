using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IEffectSubCategoryRepository : IRepository<EffectSubCategoryEntity>
    {
    }

    public class EffectSubCategoryRepository : Repository<EffectSubCategoryEntity, NaheulbookDbContext>, IEffectSubCategoryRepository
    {
        public EffectSubCategoryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}