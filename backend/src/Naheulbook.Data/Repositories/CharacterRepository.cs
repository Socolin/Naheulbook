#pragma warning disable 8619
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Extensions;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ICharacterRepository : IRepository<CharacterEntity>
{
    Task<CharacterEntity?> GetWithAllDataAsync(int id);
    Task<CharacterEntity?> GetWithGroupAsync(int id);
    Task<List<CharacterEntity>> GetForSummaryByOwnerIdAsync(int ownerId);
    Task<List<IHistoryEntry>> GetHistoryByCharacterIdAsync(int characterId, int? groupId, int page, bool isGm);
    Task<List<CharacterEntity>> SearchCharacterWithNoGroupByNameWithOriginWithOwner(string filter, int maxCount);
    Task<CharacterEntity?> GetWithGroupWithJobsWithOriginAsync(int requestCharacterId);
    Task<List<CharacterEntity>> GetWithItemsWithModifiersByGroupAndByIdAsync(int groupId, IEnumerable<int> characterIds);
}

public class CharacterRepository : Repository<CharacterEntity, NaheulbookDbContext>, ICharacterRepository
{
    private const int HistoryPageSize = 40;

    public CharacterRepository(NaheulbookDbContext context)
        : base(context)
    {
    }

    public async Task<CharacterEntity?> GetWithAllDataAsync(int id)
    {
        var character = await Context.Characters
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
            .Include(c => c.Invites)
            .ThenInclude(i => i.Group)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (character == null)
            return null;

        await Context.Entry(character)
            .Collection(c => c.Items)
            .Query()
            .IncludeItemTemplateDetails(c => c.ItemTemplate)
            .LoadAsync();

        return character;
    }

    public Task<CharacterEntity?> GetWithGroupAsync(int id)
    {
        return Context.Characters
            .Include(c => c.Group)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public Task<List<CharacterEntity>> GetForSummaryByOwnerIdAsync(int ownerId)
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

    public Task<List<CharacterEntity>> SearchCharacterWithNoGroupByNameWithOriginWithOwner(string filter, int maxCount)
    {
        return Context.Characters
            .Include(x => x.Owner)
            .Include(x => x.Origin)
            .Where(x => x.Owner.ShowInSearchUntil > DateTime.UtcNow)
            .Where(x => x.GroupId == null && x.Name.ToUpper().Contains(filter.ToUpper()) && x.IsNpc == false)
            .Take(maxCount)
            .ToListAsync();
    }

    public Task<CharacterEntity?> GetWithGroupWithJobsWithOriginAsync(int characterId)
    {
        return Context.Characters
            .Include(x => x.Group)
            .Include(x => x.Jobs)
            .ThenInclude(x => x.Job)
            .Include(x => x.Origin)
            .FirstOrDefaultAsync(x => x.Id == characterId);
    }

    public Task<List<CharacterEntity>> GetWithItemsWithModifiersByGroupAndByIdAsync(int groupId, IEnumerable<int> characterIds)
    {
        return Context.Characters
            .Include(x => x.Modifiers)
            .ThenInclude(x => x.Values)
            .Include(x => x.Items)
            .Where(x => characterIds.Contains(x.Id) && x.GroupId == groupId)
            .ToListAsync();
    }
}