using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IMonsterTemplateRepository : IRepository<MonsterTemplate>
    {
    }

    public class MonsterTemplateRepository : Repository<MonsterTemplate, NaheulbookDbContext>, IMonsterTemplateRepository
    {
        public MonsterTemplateRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}