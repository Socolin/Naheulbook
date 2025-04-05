using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IGodRepository : IRepository<GodEntity>;

public class GodRepository(NaheulbookDbContext context) : Repository<GodEntity, NaheulbookDbContext>(context), IGodRepository;