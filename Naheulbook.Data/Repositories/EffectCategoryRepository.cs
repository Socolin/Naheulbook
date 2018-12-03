using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IEffectCategoryRepository : IRepository<EffectCategory>
    {
    }

    public class EffectCategoryRepository : Repository<EffectCategory, NaheulbookDbContext>, IEffectCategoryRepository
    {
        public EffectCategoryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}