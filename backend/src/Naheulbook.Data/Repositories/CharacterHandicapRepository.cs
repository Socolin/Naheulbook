using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterHandicapRepository : IRepository<CharacterHandicapEntity>
{
    Task<CharacterHandicapEntity?> GetByCharacterIdAndHandicapIdAsync(
        int characterId,
        Guid handicapId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterHandicapRepository(NaheulbookDbContext naheulbookDbContext)
    : Repository<CharacterHandicapEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterHandicapRepository
{
    public Task<CharacterHandicapEntity?> GetByCharacterIdAndHandicapIdAsync(
        int characterId,
        Guid handicapId,
        CancellationToken cancellationToken = default
    )
    {
        return Context.CharacterHandicaps
            .Include(x => x.Handicap)
            .Where(x => x.CharacterId == characterId && x.HandicapId == handicapId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
