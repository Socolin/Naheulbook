using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IMonsterTraitRepository : IRepository<MonsterTraitEntity>
{
}

public class MonsterTraitRepository : Repository<MonsterTraitEntity, NaheulbookDbContext>, IMonsterTraitRepository
{
    public MonsterTraitRepository(NaheulbookDbContext context)
        : base(context)
    {
    }
}