using Naheulbook.Data.EntityFrameworkCore.DbContexts;

namespace Naheulbook.Data.Repositories;

public interface IAptitudeRepository : IRepository<AptitudeRepository>;

public class AptitudeRepository(NaheulbookDbContext context)
    : Repository<AptitudeRepository, NaheulbookDbContext>(context), IAptitudeRepository
{
}