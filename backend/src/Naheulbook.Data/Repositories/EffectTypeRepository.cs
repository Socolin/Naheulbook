using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IEffectTypeRepository : IRepository<EffectTypeEntity>
{
}


public class EffectTypeRepository(NaheulbookDbContext naheulbookDbContext) : Repository<EffectTypeEntity, NaheulbookDbContext>(naheulbookDbContext), IEffectTypeRepository;