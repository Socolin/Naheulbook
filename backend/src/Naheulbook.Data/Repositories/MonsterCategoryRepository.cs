using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMonsterSubCategoryRepository : IRepository<MonsterSubCategoryEntity>;

public class MonsterSubCategoryRepository(NaheulbookDbContext context) : Repository<MonsterSubCategoryEntity, NaheulbookDbContext>(context), IMonsterSubCategoryRepository;