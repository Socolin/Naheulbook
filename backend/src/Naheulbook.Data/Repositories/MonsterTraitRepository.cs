using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMonsterTraitRepository : IRepository<MonsterTraitEntity>;

public class MonsterTraitRepository(NaheulbookDbContext context) : Repository<MonsterTraitEntity, NaheulbookDbContext>(context), IMonsterTraitRepository;