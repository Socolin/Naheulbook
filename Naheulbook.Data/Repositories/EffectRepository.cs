using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IEffectRepository : IRepository<Effect>
    {
        Task<ICollection<Effect>> GetByCategoryWithModifiersAsync(long categoryId);
    }

    public class EffectRepository : Repository<Effect, NaheulbookDbContext>, IEffectRepository
    {
        public EffectRepository(NaheulbookDbContext naheulbookDbContext)
            : base(naheulbookDbContext)
        {
        }

        public async Task<ICollection<Effect>> GetByCategoryWithModifiersAsync(long categoryId)
        {
            return await Context.Effects
                .Where(e => e.CategoryId == categoryId)
                .Include(e => e.Modifiers)
                .ToListAsync();
        }
    }
}