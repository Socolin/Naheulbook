using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterHistoryEntryRepository : IRepository<CharacterHistoryEntryEntity>;

public class CharacterHistoryEntryRepository(NaheulbookDbContext context) : Repository<CharacterHistoryEntryEntity, NaheulbookDbContext>(context), ICharacterHistoryEntryRepository;