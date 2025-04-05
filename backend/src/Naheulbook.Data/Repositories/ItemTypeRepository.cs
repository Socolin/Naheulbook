using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IItemTypeRepository : IRepository<ItemTypeEntity>;

public class ItemTypeRepository(NaheulbookDbContext context) : Repository<ItemTypeEntity, NaheulbookDbContext>(context), IItemTypeRepository;