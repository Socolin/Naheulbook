using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMonsterTypeRepository : IRepository<MonsterTypeEntity>
{
    Task<List<MonsterTypeEntity>> GetAllWithCategoriesAsync();
}

public class MonsterTypeRepository(NaheulbookDbContext context) : Repository<MonsterTypeEntity, NaheulbookDbContext>(context), IMonsterTypeRepository
{
    public Task<List<MonsterTypeEntity>> GetAllWithCategoriesAsync()
    {
        return Context.MonsterTypes
            .Include(x => x.SubCategories)
            .ToListAsync();
    }
}