using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ISkillRepository : IRepository<Skill>
    {
        Task<ICollection<Skill>> GetAllWithEffectsAsync();
    }

    public class SkillRepository : Repository<Skill, NaheulbookDbContext>, ISkillRepository
    {
        public SkillRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public async Task<ICollection<Skill>> GetAllWithEffectsAsync()
        {
            return await Context.Skills
                .Include(s => s.SkillEffects)
                .ToListAsync();
        }
    }
}