using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ICharacterSkillRepository : IRepository<CharacterSkill>
    {
    }

    public class CharacterSkillRepository : Repository<CharacterSkill, NaheulbookDbContext>, ICharacterSkillRepository
    {
        public CharacterSkillRepository(NaheulbookDbContext naheulbookDbContext)
            : base(naheulbookDbContext)
        {
        }
    }
}