#pragma warning disable 8619
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ICharacterModifierRepository : IRepository<CharacterModifierEntity>
{
    Task<CharacterModifierEntity?> GetByIdAndCharacterIdAsync(int characterId, int characterModifierId);
}

public class CharacterModifierRepository : Repository<CharacterModifierEntity, NaheulbookDbContext>, ICharacterModifierRepository
{
    public CharacterModifierRepository(NaheulbookDbContext naheulbookDbContext)
        : base(naheulbookDbContext)
    {
    }

    public Task<CharacterModifierEntity?> GetByIdAndCharacterIdAsync(int characterId, int characterModifierId)
    {
        return Context.CharacterModifiers
            .Include(x => x.Values)
            .SingleOrDefaultAsync(m => m.CharacterId == characterId && m.Id == characterModifierId);
    }
}