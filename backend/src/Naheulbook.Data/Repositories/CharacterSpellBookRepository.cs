using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterSpellBookRepository : IRepository<CharacterSpellBookEntity>
{
    Task<CharacterSpellBookEntity?> GetByCharacterIdAndSpellBookIdAsync(
        int characterId,
        Guid spellBookId,
        CancellationToken cancellationToken = default
    );
}

public class CharacterSpellBookRepository(NaheulbookDbContext naheulbookDbContext)
    : Repository<CharacterSpellBookEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterSpellBookRepository
{
    public Task<CharacterSpellBookEntity?> GetByCharacterIdAndSpellBookIdAsync(
        int characterId,
        Guid spellBookId,
        CancellationToken cancellationToken = default
    )
    {
        return Context.CharacterSpellBooks
            .Include(x => x.SpellBook)
            .Where(x => x.CharacterId == characterId && x.SpellBookId == spellBookId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
