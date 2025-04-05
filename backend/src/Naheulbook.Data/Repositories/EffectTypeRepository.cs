using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IEffectTypeRepository : IRepository<EffectTypeEntity>;


public class EffectTypeRepository(NaheulbookDbContext naheulbookDbContext) : Repository<EffectTypeEntity, NaheulbookDbContext>(naheulbookDbContext), IEffectTypeRepository;