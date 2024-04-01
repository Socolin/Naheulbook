using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IItemTypeRepository : IRepository<ItemTypeEntity>
{
}

public class ItemTypeRepository(NaheulbookDbContext context) : Repository<ItemTypeEntity, NaheulbookDbContext>(context), IItemTypeRepository;