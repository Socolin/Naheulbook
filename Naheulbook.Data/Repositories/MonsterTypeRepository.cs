using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterTypeRepository : IRepository<MonsterType>
    {
    }

    public class MonsterTypeRepository : Repository<MonsterType, NaheulbookDbContext>, IMonsterTypeRepository
    {
        public MonsterTypeRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}