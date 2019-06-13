using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterTypeRepository : IRepository<MonsterType>
    {
        Task<List<MonsterType>> GetAllWithCategoriesAsync();
    }

    public class MonsterTypeRepository : Repository<MonsterType, NaheulbookDbContext>, IMonsterTypeRepository
    {
        public MonsterTypeRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<MonsterType>> GetAllWithCategoriesAsync()
        {
            return Context.MonsterTypes
                .Include(x => x.Categories)
                .ToListAsync();
        }
    }
}