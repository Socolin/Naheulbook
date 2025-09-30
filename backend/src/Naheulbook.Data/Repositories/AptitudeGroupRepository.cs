using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IAptitudeGroupRepository : IRepository<AptitudeGroupEntity>
{
    Task<AptitudeGroupEntity?> GetByIdWithAptitudesAsync(
        Guid aptitudeGroupId,
        CancellationToken cancellationToken = default
    );
}

public class AptitudeGroupRepository(NaheulbookDbContext context)
    : Repository<AptitudeGroupEntity, NaheulbookDbContext>(context), IAptitudeGroupRepository
{
    public async Task<AptitudeGroupEntity?> GetByIdWithAptitudesAsync(
        Guid aptitudeGroupId,
        CancellationToken cancellationToken = default
    )
    {
        return await Context.AptitudeGroups
            .Where(x => x.Id == aptitudeGroupId)
            .Include(x => x.Aptitudes)
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }
}