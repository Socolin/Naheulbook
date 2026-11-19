using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterTalentRepository : IRepository<CharacterTalentEntity>
{
    Task<CharacterTalentEntity?> GetByCharacterIdAndTalentIdAsync(
        int characterId,
        Guid talentId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterTalentRepository(NaheulbookDbContext naheulbookDbContext)
    : Repository<CharacterTalentEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterTalentRepository
{
    public Task<CharacterTalentEntity?> GetByCharacterIdAndTalentIdAsync(
        int characterId,
        Guid talentId,
        CancellationToken cancellationToken = default
    )
    {
        return Context.CharacterTalents
            .Include(x => x.Talent)
            .Where(x => x.CharacterId == characterId && x.TalentId == talentId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
