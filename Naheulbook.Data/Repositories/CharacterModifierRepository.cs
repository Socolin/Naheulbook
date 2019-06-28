using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ICharacterModifierRepository : IRepository<CharacterModifier>
    {
    }

    public class CharacterModifierRepository : Repository<CharacterModifier, NaheulbookDbContext>, ICharacterModifierRepository
    {
        public CharacterModifierRepository(NaheulbookDbContext naheulbookDbContext)
            : base(naheulbookDbContext)
        {
        }
    }
}