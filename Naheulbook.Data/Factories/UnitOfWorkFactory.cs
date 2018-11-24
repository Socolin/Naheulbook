using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Data.Factories
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly DbContextOptions<NaheulbookDbContext> _naheulbookDbContextOptions;

        public UnitOfWorkFactory(DbContextOptions<NaheulbookDbContext> naheulbookDbContextOptions)
        {
            _naheulbookDbContextOptions = naheulbookDbContextOptions;
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return new UnitOfWork(new NaheulbookDbContext(_naheulbookDbContextOptions));
        }
    }
}