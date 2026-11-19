using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterMasteryRepository : IRepository<CharacterMasteryEntity>
{
    Task<CharacterMasteryEntity?> GetByCharacterIdAndMasteryIdAsync(
        int characterId,
        Guid masteryId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterMasteryRepository(NaheulbookDbContext naheulbookDbContext)
    : Repository<CharacterMasteryEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterMasteryRepository
{
    public Task<CharacterMasteryEntity?> GetByCharacterIdAndMasteryIdAsync(
        int characterId,
        Guid masteryId,
        CancellationToken cancellationToken = default
    )
    {
        return Context.CharacterMasteries
            .Include(x => x.Mastery)
            .Where(x => x.CharacterId == characterId && x.MasteryId == masteryId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
