using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ICharacterSkillRepository : IRepository<CharacterSkillEntity>
{
}

public class CharacterSkillRepository : Repository<CharacterSkillEntity, NaheulbookDbContext>, ICharacterSkillRepository
{
    public CharacterSkillRepository(NaheulbookDbContext naheulbookDbContext)
        : base(naheulbookDbContext)
    {
    }
}