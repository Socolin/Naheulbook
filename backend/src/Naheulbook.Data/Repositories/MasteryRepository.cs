using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMasteryRepository : IRepository<MasteryEntity>;

public class MasteryRepository(NaheulbookDbContext context)
    : Repository<MasteryEntity, NaheulbookDbContext>(context), IMasteryRepository;
