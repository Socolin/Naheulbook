using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

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