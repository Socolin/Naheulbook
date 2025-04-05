using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IEffectSubCategoryRepository : IRepository<EffectSubCategoryEntity>;

public class EffectSubCategoryRepository(NaheulbookDbContext context) : Repository<EffectSubCategoryEntity, NaheulbookDbContext>(context), IEffectSubCategoryRepository;