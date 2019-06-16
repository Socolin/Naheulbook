using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ICharacterRepository : IRepository<Character>
    {
    }

    public class CharacterRepository : Repository<Character, NaheulbookDbContext>, ICharacterRepository
    {
        public CharacterRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}