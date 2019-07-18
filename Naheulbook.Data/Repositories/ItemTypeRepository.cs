using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IItemTypeRepository : IRepository<ItemType>
    {
    }

    public class ItemTypeRepository : Repository<ItemType, NaheulbookDbContext>, IItemTypeRepository
    {
        public ItemTypeRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}