using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterTraitRepository : IRepository<MonsterTrait>
    {
    }

    public class MonsterTraitRepository : Repository<MonsterTrait, NaheulbookDbContext>, IMonsterTraitRepository
    {
        public MonsterTraitRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}