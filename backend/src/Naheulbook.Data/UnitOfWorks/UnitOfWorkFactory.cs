using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;

namespace Naheulbook.Data.UnitOfWorks;

public interface IUnitOfWorkFactory
{
    IUnitOfWork CreateUnitOfWork();
}

public class UnitOfWorkFactory(DbContextOptions<NaheulbookDbContext> naheulbookDbContextOptions)
    : IUnitOfWorkFactory
{
    public IUnitOfWork CreateUnitOfWork()
    {
        return new UnitOfWork(new NaheulbookDbContext(naheulbookDbContextOptions));
    }
}