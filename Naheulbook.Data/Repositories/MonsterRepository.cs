using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterRepository : IRepository<Monster>
    {
    }

    public class MonsterRepository : Repository<Monster, NaheulbookDbContext>, IMonsterRepository
    {
        public MonsterRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}