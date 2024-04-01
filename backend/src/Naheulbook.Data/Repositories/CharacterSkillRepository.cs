using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ICharacterSkillRepository : IRepository<CharacterSkillEntity>
{
}

public class CharacterSkillRepository(NaheulbookDbContext naheulbookDbContext) : Repository<CharacterSkillEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterSkillRepository;