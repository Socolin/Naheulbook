using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ICharacterHistoryEntryRepository : IRepository<CharacterHistoryEntryEntity>;

public class CharacterHistoryEntryRepository(NaheulbookDbContext context) : Repository<CharacterHistoryEntryEntity, NaheulbookDbContext>(context), ICharacterHistoryEntryRepository;