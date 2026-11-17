using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ISkillRepository : IRepository<SkillEntity>
{
    Task<ICollection<SkillEntity>> GetAllWithEffectsAsync(CancellationToken cancellationToken = default);
}

public class SkillRepository(NaheulbookDbContext context) : Repository<SkillEntity, NaheulbookDbContext>(context), ISkillRepository
{
    public async Task<ICollection<SkillEntity>> GetAllWithEffectsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Skills
            .Include(s => s.SkillEffects)
            .ToListAsync(cancellationToken);
    }
}