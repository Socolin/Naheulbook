using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IGroupHistoryEntryRepository : IRepository<GroupHistoryEntryEntity>
{
    Task<List<GroupHistoryEntryEntity>> GetByGroupIdAndPageAsync(int groupId, int page);
}

public class GroupHistoryEntryRepository : Repository<GroupHistoryEntryEntity, NaheulbookDbContext>, IGroupHistoryEntryRepository
{
    private const int PageSize = 40;

    public GroupHistoryEntryRepository(NaheulbookDbContext naheulbookDbContext)
        : base(naheulbookDbContext)
    {
    }

    public Task<List<GroupHistoryEntryEntity>> GetByGroupIdAndPageAsync(int groupId, int page)
    {
        return Context.GroupHistory
            .Where(h => h.GroupId == groupId)
            .Skip(PageSize * page)
            .Take(PageSize)
            .ToListAsync();
    }
}