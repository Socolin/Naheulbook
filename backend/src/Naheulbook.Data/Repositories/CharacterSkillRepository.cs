using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICharacterSkillRepository : IRepository<CharacterSkillEntity>;

public class CharacterSkillRepository(NaheulbookDbContext naheulbookDbContext) : Repository<CharacterSkillEntity, NaheulbookDbContext>(naheulbookDbContext), ICharacterSkillRepository;