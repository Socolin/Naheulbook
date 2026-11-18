#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface ICharacterRepository : IRepository<CharacterEntity>
{
    Task<CharacterEntity?> GetWithAllDataAsync(int id, CancellationToken cancellationToken = default);
    Task<CharacterEntity?> GetWithGroupAsync(int id, CancellationToken cancellationToken = default);
    Task<CharacterEntity?> GetWithGroupWithOriginAsync(int id, CancellationToken cancellationToken = default);
    Task<List<CharacterEntity>> GetForSummaryByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<List<IHistoryEntry>> GetHistoryByCharacterIdAsync(int characterId, int? groupId, int page, bool isGm, CancellationToken cancellationToken = default);
    Task<List<CharacterEntity>> SearchCharacterWithNoGroupByNameWithOriginWithOwner(string filter, int maxCount, CancellationToken cancellationToken = default);
    Task<CharacterEntity?> GetWithGroupWithJobsWithOriginAsync(int requestCharacterId, CancellationToken cancellationToken = default);
    Task<List<CharacterEntity>> GetWithItemsWithModifiersByGroupAndByIdAsync(int groupId, IEnumerable<int> characterIds, CancellationToken cancellationToken = default);
}

public class CharacterRepository(NaheulbookDbContext context) : Repository<CharacterEntity, NaheulbookDbContext>(context), ICharacterRepository
{
    private const int HistoryPageSize = 40;

    public async Task<CharacterEntity?> GetWithAllDataAsync(int id, CancellationToken cancellationToken = default)
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
            .Include(c => c.Aptitudes)
            .ThenInclude(i => i.Aptitude)
            .Include(c => c.SpecialAttacks)
            .ThenInclude(i => i.SpecialAttack)
            .Include(c => c.Jobs)
            .Include(c => c.Invites)
            .ThenInclude(i => i.Group)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (character == null)
            return null;

        await Context.Entry(character)
            .Collection(c => c.Items)
            .Query()
            .IncludeItemTemplateDetails(c => c.ItemTemplate)
            .LoadAsync(cancellationToken);

        return character;
    }

    public Task<CharacterEntity?> GetWithGroupAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Characters
            .Include(c => c.Group)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<CharacterEntity?> GetWithGroupWithOriginAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Characters
            .Include(c => c.Group)
            .Include(c => c.Origin)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<CharacterEntity>> GetForSummaryByOwnerIdAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        return Context.Characters
            .Where(x => x.OwnerId == ownerId)
            .Include(x => x.Origin)
            .Include(x => x.Jobs)
            .ThenInclude(x => x.Job)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<IHistoryEntry>> GetHistoryByCharacterIdAsync(int characterId, int? groupId, int page, bool isGm, CancellationToken cancellationToken = default)
    {
        var history = new List<IHistoryEntry>();
        var characterHistory = await Context.CharacterHistory
            .Where(x => x.CharacterId == characterId)
            .Where(x => x.Gm && isGm || !x.Gm)
            .OrderByDescending(x => x.Date)
            .Skip(HistoryPageSize * page)
            .Take(HistoryPageSize)
            .ToListAsync(cancellationToken);
        history.AddRange(characterHistory);

        if (characterHistory.Count > 0 && groupId.HasValue)
        {
            var groupHistory = await Context.GroupHistory
                .Where(x => x.GroupId == groupId.Value)
                .Where(x => x.Date >= characterHistory.Last().Date)
                .Where(x => page == 0 || x.Date < characterHistory.First().Date)
                .OrderByDescending(x => x.Date)
                .ToListAsync(cancellationToken);

            history.AddRange(groupHistory);
        }

        history.Sort((h1, h2) => h2.Date.CompareTo(h1.Date));

        return history;
    }

    public Task<List<CharacterEntity>> SearchCharacterWithNoGroupByNameWithOriginWithOwner(string filter, int maxCount, CancellationToken cancellationToken = default)
    {
        return Context.Characters
            .Include(x => x.Owner)
            .Include(x => x.Origin)
            .Where(x => x.Owner.ShowInSearchUntil > DateTime.UtcNow)
            .Where(x => x.GroupId == null && x.Name.ToUpper().Contains(filter.ToUpper()) && x.IsNpc == false)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public Task<CharacterEntity?> GetWithGroupWithJobsWithOriginAsync(int characterId, CancellationToken cancellationToken = default)
    {
        return Context.Characters
            .Include(x => x.Group)
            .Include(x => x.Jobs)
            .ThenInclude(x => x.Job)
            .Include(x => x.Origin)
            .FirstOrDefaultAsync(x => x.Id == characterId, cancellationToken);
    }

    public Task<List<CharacterEntity>> GetWithItemsWithModifiersByGroupAndByIdAsync(int groupId, IEnumerable<int> characterIds, CancellationToken cancellationToken = default)
    {
        return Context.Characters
            .Include(x => x.Modifiers)
            .ThenInclude(x => x.Values)
            .Include(x => x.Items)
            .Include(x => x.SpecialAttacks)
            .ThenInclude(x => x.SpecialAttack)
            .Where(x => characterIds.Contains(x.Id) && x.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }
}