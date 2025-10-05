using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IAptitudeRepository : IRepository<AptitudeEntity>;

public class AptitudeRepository(NaheulbookDbContext context)
    : Repository<AptitudeEntity, NaheulbookDbContext>(context), IAptitudeRepository
{
}