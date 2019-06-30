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
        Task<Character> GetWithGroupAsync(int id);
        Task<List<Character>> GetForSummaryByOwnerIdAsync(int ownerId);
        Task<List<IHistoryEntry>> GetHistoryByCharacterIdAsync(int characterId, int? groupId, int page, bool isGm);
        Task<List<Character>> SearchCharacterWithNoGroupByNameWithOriginWithOwner(string filter, int maxCount);
        Task<Character> GetWithOriginWithJobsAsync(int requestCharacterId);
    }

    public class CharacterRepository : Repository<Character, NaheulbookDbContext>, ICharacterRepository
    {
        private const int HistoryPageSize = 40;

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
                .Include(c => c.Items)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.UnSkills)
                .Include(c => c.Items)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Skills)
                .Include(c => c.Items)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Modifiers)
                .Include(c => c.Items)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Requirements)
                .Include(c => c.Items)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.Slots)
                .ThenInclude(s => s.Slot)
                .Include(c => c.Items)
                .ThenInclude(i => i.ItemTemplate)
                .ThenInclude(i => i.SkillModifiers)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Character> GetWithGroupAsync(int id)
        {
            return Context.Characters
                .Include(c => c.Group)
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

        public async Task<List<IHistoryEntry>> GetHistoryByCharacterIdAsync(int characterId, int? groupId, int page, bool isGm)
        {
            var history = new List<IHistoryEntry>();
            var characterHistory = await Context.CharacterHistory
                .Include(x => x.Item)
                .Include(x => x.Effect)
                .Include(x => x.CharacterModifier)
                .Where(x => x.CharacterId == characterId)
                .Where(x => x.Gm && isGm || !x.Gm)
                .OrderByDescending(x => x.Date)
                .Skip(HistoryPageSize * page)
                .Take(HistoryPageSize)
                .ToListAsync();
            history.AddRange(characterHistory);

            if (characterHistory.Count > 0 && groupId.HasValue)
            {
                var groupHistory = await Context.GroupHistory
                    .Where(x => x.GroupId == groupId.Value)
                    .Where(x => x.Date >= characterHistory.Last().Date)
                    .Where(x => page == 0 || x.Date < characterHistory.First().Date)
                    .OrderByDescending(x => x.Date)
                    .ToListAsync();

                history.AddRange(groupHistory);
            }

            history.Sort((h1, h2) => h2.Date.CompareTo(h1.Date));

            return history;
        }

        public Task<List<Character>> SearchCharacterWithNoGroupByNameWithOriginWithOwner(string filter, int maxCount)
        {
            return Context.Characters
                .Include(x => x.Owner)
                .Include(x => x.Origin)
                .Where(x => x.GroupId == null && x.Name.ToUpper().Contains(filter.ToUpper()) && x.IsNpc == false)
                .Take(maxCount)
                .ToListAsync();
        }

        public Task<Character> GetWithOriginWithJobsAsync(int characterId)
        {
            return Context.Characters
                .Include(x => x.Jobs)
                .ThenInclude(x => x.Job)
                .Include(x => x.Origin)
                .FirstOrDefaultAsync(x => x.Id == characterId);
        }
    }
}