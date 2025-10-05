using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterAptitudeRepository : IRepository<CharacterAptitudeEntity>
{
    Task<CharacterAptitudeEntity?> GetByCharacterIdAndAptitudeIdAsync(
        int characterId,
        Guid aptitudeId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterAptitudeRepository(NaheulbookDbContext naheulbookDbContext)
    : Repository<CharacterAptitudeEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterAptitudeRepository
{
    public Task<CharacterAptitudeEntity?> GetByCharacterIdAndAptitudeIdAsync(
        int characterId,
        Guid aptitudeId,
        CancellationToken cancellationToken = default
    )
    {
        return Context.CharacterAptitudes
            .Include(x => x.Aptitude)
            .Where(x => x.CharacterId == characterId && x.AptitudeId == aptitudeId)
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }
}