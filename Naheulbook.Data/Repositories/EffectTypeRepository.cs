using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IEffectTypeRepository : IRepository<EffectType>
    {
    }


    public class EffectTypeRepository : Repository<EffectType, NaheulbookDbContext>, IEffectTypeRepository
    {
        public EffectTypeRepository(NaheulbookDbContext naheulbookDbContext)
            : base(naheulbookDbContext)
        {
        }
    }
}