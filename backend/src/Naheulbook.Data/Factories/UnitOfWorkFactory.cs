using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Extensions.UnitOfWorks;

namespace Naheulbook.Data.Factories;

public class UnitOfWorkFactory(DbContextOptions<NaheulbookDbContext> naheulbookDbContextOptions)
    : IUnitOfWorkFactory
{
    public IUnitOfWork CreateUnitOfWork()
    {
        return new UnitOfWork(new NaheulbookDbContext(naheulbookDbContextOptions));
    }
}