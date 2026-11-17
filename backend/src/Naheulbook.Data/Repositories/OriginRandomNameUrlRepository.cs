#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IOriginRandomNameUrlRepository : IRepository<OriginRandomNameUrlEntity>
{
    Task<OriginRandomNameUrlEntity?> GetByOriginIdAndSexAsync(string sex, Guid originId, CancellationToken cancellationToken = default);
}

public class OriginRandomNameUrlRepository(NaheulbookDbContext context) : Repository<OriginRandomNameUrlEntity, NaheulbookDbContext>(context), IOriginRandomNameUrlRepository
{
    public Task<OriginRandomNameUrlEntity?> GetByOriginIdAndSexAsync(string sex, Guid originId, CancellationToken cancellationToken = default)
    {
        return Context.OriginRandomNameUrls
            .SingleOrDefaultAsync(x => x.Sex == sex && x.OriginId == originId, cancellationToken);
    }
}