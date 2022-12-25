using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ISkillRepository : IRepository<SkillEntity>
    {
        Task<ICollection<SkillEntity>> GetAllWithEffectsAsync();
    }

    public class SkillRepository : Repository<SkillEntity, NaheulbookDbContext>, ISkillRepository
    {
        public SkillRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public async Task<ICollection<SkillEntity>> GetAllWithEffectsAsync()
        {
            return await Context.Skills
                .Include(s => s.SkillEffects)
                .ToListAsync();
        }
    }
}