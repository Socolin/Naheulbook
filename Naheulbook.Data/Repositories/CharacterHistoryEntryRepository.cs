using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ICharacterHistoryEntryRepository : IRepository<CharacterHistoryEntryEntity>
{
}

public class CharacterHistoryEntryRepository : Repository<CharacterHistoryEntryEntity, NaheulbookDbContext>, ICharacterHistoryEntryRepository
{
    public CharacterHistoryEntryRepository(NaheulbookDbContext context)
        : base(context)
    {
    }
}