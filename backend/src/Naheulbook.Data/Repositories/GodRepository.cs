using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IGodRepository : IRepository<GodEntity>;

public class GodRepository(NaheulbookDbContext context) : Repository<GodEntity, NaheulbookDbContext>(context), IGodRepository;