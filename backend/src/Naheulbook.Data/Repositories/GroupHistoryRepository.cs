using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IGroupHistoryEntryRepository : IRepository<GroupHistoryEntryEntity>
{
    Task<List<GroupHistoryEntryEntity>> GetByGroupIdAndPageAsync(int groupId, int page, CancellationToken cancellationToken = default);
}

public class GroupHistoryEntryRepository(NaheulbookDbContext naheulbookDbContext) : Repository<GroupHistoryEntryEntity, NaheulbookDbContext>(naheulbookDbContext), IGroupHistoryEntryRepository
{
    private const int PageSize = 40;

    public Task<List<GroupHistoryEntryEntity>> GetByGroupIdAndPageAsync(int groupId, int page, CancellationToken cancellationToken = default)
    {
        return Context.GroupHistory
            .Where(h => h.GroupId == groupId)
            .OrderByDescending(h => h.Date)
            .ThenByDescending(h => h.Id)
            .Skip(PageSize * page)
            .Take(PageSize)
            .ToListAsync(cancellationToken);
    }
}