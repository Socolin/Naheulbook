using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterCategoryRepository : IRepository<MonsterCategory>
    {
    }

    public class MonsterCategoryRepository : Repository<MonsterCategory, NaheulbookDbContext>, IMonsterCategoryRepository
    {
        public MonsterCategoryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}