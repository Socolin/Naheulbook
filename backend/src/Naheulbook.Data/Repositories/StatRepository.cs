using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IStatRepository : IRepository<StatEntity>
{
}

public class StatRepository(NaheulbookDbContext context) : Repository<StatEntity, NaheulbookDbContext>(context), IStatRepository;