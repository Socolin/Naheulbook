using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterSpecialAttackRepository : IRepository<CharacterSpecialAttackEntity>
{
    Task<CharacterSpecialAttackEntity?> GetByCharacterIdAndSpecialAttackIdAsync(
        int characterId,
        Guid specialAttackId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterSpecialAttackRepository(NaheulbookDbContext naheulbookDbContext)
    : Repository<CharacterSpecialAttackEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterSpecialAttackRepository
{
    public Task<CharacterSpecialAttackEntity?> GetByCharacterIdAndSpecialAttackIdAsync(
        int characterId,
        Guid specialAttackId,
        CancellationToken cancellationToken = default
    )
    {
        return Context.CharacterSpecialAttacks
            .Include(x => x.SpecialAttack)
            .Where(x => x.CharacterId == characterId && x.SpecialAttackId == specialAttackId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
