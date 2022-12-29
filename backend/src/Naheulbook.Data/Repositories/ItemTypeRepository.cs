using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IItemTypeRepository : IRepository<ItemTypeEntity>
{
}

public class ItemTypeRepository : Repository<ItemTypeEntity, NaheulbookDbContext>, IItemTypeRepository
{
    public ItemTypeRepository(NaheulbookDbContext context)
        : base(context)
    {
    }
}