using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IMonsterSubCategoryRepository : IRepository<MonsterSubCategoryEntity>
{
}

public class MonsterSubCategoryRepository : Repository<MonsterSubCategoryEntity, NaheulbookDbContext>, IMonsterSubCategoryRepository
{
    public MonsterSubCategoryRepository(NaheulbookDbContext context)
        : base(context)
    {
    }
}