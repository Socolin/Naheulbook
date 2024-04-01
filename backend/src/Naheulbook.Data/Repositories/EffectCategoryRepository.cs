using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IEffectSubCategoryRepository : IRepository<EffectSubCategoryEntity>
{
}

public class EffectSubCategoryRepository(NaheulbookDbContext context) : Repository<EffectSubCategoryEntity, NaheulbookDbContext>(context), IEffectSubCategoryRepository;