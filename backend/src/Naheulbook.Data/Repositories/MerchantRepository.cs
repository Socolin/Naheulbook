using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IMerchantRepository : IRepository<MerchantEntity>
{
}

public class MerchantRepository(NaheulbookDbContext context)
    : Repository<MerchantEntity, NaheulbookDbContext>(context), IMerchantRepository
{
}