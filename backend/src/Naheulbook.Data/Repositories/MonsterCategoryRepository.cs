using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IMonsterSubCategoryRepository : IRepository<MonsterSubCategoryEntity>
{
}

public class MonsterSubCategoryRepository(NaheulbookDbContext context) : Repository<MonsterSubCategoryEntity, NaheulbookDbContext>(context), IMonsterSubCategoryRepository;