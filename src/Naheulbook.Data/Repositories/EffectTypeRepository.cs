using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IEffectTypeRepository : IRepository<EffectTypeEntity>
{
}


public class EffectTypeRepository : Repository<EffectTypeEntity, NaheulbookDbContext>, IEffectTypeRepository
{
    public EffectTypeRepository(NaheulbookDbContext naheulbookDbContext)
        : base(naheulbookDbContext)
    {
    }
}