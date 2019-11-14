using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterSubCategoryRepository : IRepository<MonsterSubCategory>
    {
    }

    public class MonsterSubCategoryRepository : Repository<MonsterSubCategory, NaheulbookDbContext>, IMonsterSubCategoryRepository
    {
        public MonsterSubCategoryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}