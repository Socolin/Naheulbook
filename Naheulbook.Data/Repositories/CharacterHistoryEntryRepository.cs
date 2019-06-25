using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ICharacterHistoryEntryRepository : IRepository<CharacterHistoryEntry>
    {
    }

    public class CharacterHistoryEntryRepository : Repository<CharacterHistoryEntry, NaheulbookDbContext>, ICharacterHistoryEntryRepository
    {
        public CharacterHistoryEntryRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}