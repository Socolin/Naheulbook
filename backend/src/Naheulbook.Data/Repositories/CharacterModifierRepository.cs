#pragma warning disable 8619
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterModifierRepository : IRepository<CharacterModifierEntity>
{
    Task<CharacterModifierEntity?> GetByIdAndCharacterIdAsync(int characterId, int characterModifierId);
}

public class CharacterModifierRepository(NaheulbookDbContext naheulbookDbContext) : Repository<CharacterModifierEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterModifierRepository
{
    public Task<CharacterModifierEntity?> GetByIdAndCharacterIdAsync(int characterId, int characterModifierId)
    {
        return Context.CharacterModifiers
            .Include(x => x.Values)
            .SingleOrDefaultAsync(m => m.CharacterId == characterId && m.Id == characterModifierId);
    }
}