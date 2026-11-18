using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ISpecialAttackRepository : IRepository<SpecialAttackEntity>;

public class SpecialAttackRepository(NaheulbookDbContext context)
    : Repository<SpecialAttackEntity, NaheulbookDbContext>(context), ISpecialAttackRepository;
