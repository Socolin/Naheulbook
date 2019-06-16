using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ICharacterRepository : IRepository<Character>
    {
        Task<Character> GetWithAllDataAsync(int id);
        Task<List<Character>> GetForSummaryByOwnerIdAsync(int ownerId);
    }

    public class CharacterRepository : Repository<Character, NaheulbookDbContext>, ICharacterRepository
    {
        public CharacterRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<Character> GetWithAllDataAsync(int id)
        {
            return Context.Characters
                .Include(c => c.Modifiers)
                .ThenInclude(c => c.Values)
                .Include(c => c.Skills)
                .Include(c => c.Group)
                .Include(c => c.Specialities)
                .ThenInclude(s => s.Speciality)
                .ThenInclude(s => s.Specials)
                .Include(c => c.Specialities)
                .ThenInclude(s => s.Speciality)
                .ThenInclude(s => s.Modifiers)
                .Include(c => c.Jobs)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<List<Character>> GetForSummaryByOwnerIdAsync(int ownerId)
        {
            return Context.Characters
                .Where(x => x.OwnerId == ownerId)
                .Include(x => x.Origin)
                .Include(x => x.Jobs)
                .ThenInclude(x => x.Job)
                .ToListAsync();
        }
    }
}