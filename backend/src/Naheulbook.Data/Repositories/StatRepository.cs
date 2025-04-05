using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IStatRepository : IRepository<StatEntity>;

public class StatRepository(NaheulbookDbContext context) : Repository<StatEntity, NaheulbookDbContext>(context), IStatRepository;