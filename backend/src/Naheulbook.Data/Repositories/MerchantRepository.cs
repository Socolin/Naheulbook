using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IMerchantRepository : IRepository<MerchantEntity>
{
}

public class MerchantRepository(NaheulbookDbContext context)
    : Repository<MerchantEntity, NaheulbookDbContext>(context), IMerchantRepository
{
}