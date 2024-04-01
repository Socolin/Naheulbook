using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IMonsterTraitRepository : IRepository<MonsterTraitEntity>
{
}

public class MonsterTraitRepository(NaheulbookDbContext context) : Repository<MonsterTraitEntity, NaheulbookDbContext>(context), IMonsterTraitRepository;