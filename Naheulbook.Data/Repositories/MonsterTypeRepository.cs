using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterTypeRepository : IRepository<MonsterTypeEntity>
    {
        Task<List<MonsterTypeEntity>> GetAllWithCategoriesAsync();
    }

    public class MonsterTypeRepository : Repository<MonsterTypeEntity, NaheulbookDbContext>, IMonsterTypeRepository
    {
        public MonsterTypeRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<MonsterTypeEntity>> GetAllWithCategoriesAsync()
        {
            return Context.MonsterTypes
                .Include(x => x.SubCategories)
                .ToListAsync();
        }
    }
}