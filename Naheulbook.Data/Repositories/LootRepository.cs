using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ILootRepository : IRepository<Loot>
    {
    }

    public class LootRepository : Repository<Loot, NaheulbookDbContext>, ILootRepository
    {
        public LootRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}