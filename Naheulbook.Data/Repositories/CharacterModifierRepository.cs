using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ICharacterModifierRepository : IRepository<CharacterModifier>
    {
        Task<CharacterModifier> GetByIdAndCharacterIdAsync(int characterId, int characterModifierId);
    }

    public class CharacterModifierRepository : Repository<CharacterModifier, NaheulbookDbContext>, ICharacterModifierRepository
    {
        public CharacterModifierRepository(NaheulbookDbContext naheulbookDbContext)
            : base(naheulbookDbContext)
        {
        }

        public Task<CharacterModifier> GetByIdAndCharacterIdAsync(int characterId, int characterModifierId)
        {
            return Context.CharacterModifiers
                .SingleOrDefaultAsync(m => m.CharacterId == characterId && m.Id == characterModifierId);
        }
    }
}