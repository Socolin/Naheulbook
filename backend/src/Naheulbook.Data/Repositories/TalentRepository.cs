using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ITalentRepository : IRepository<TalentEntity>;

public class TalentRepository(NaheulbookDbContext context)
    : Repository<TalentEntity, NaheulbookDbContext>(context), ITalentRepository;
