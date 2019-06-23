using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IGroupHistoryRepository : IRepository<GroupHistory>
    {
        Task<List<GroupHistory>> GetByGroupIdAndPageAsync(int groupId, int page);
    }

    public class GroupHistoryRepository : Repository<GroupHistory, NaheulbookDbContext>, IGroupHistoryRepository
    {
        private const int PageSize = 40;

        public GroupHistoryRepository(NaheulbookDbContext naheulbookDbContext)
            : base(naheulbookDbContext)
        {
        }

        public Task<List<GroupHistory>> GetByGroupIdAndPageAsync(int groupId, int page)
        {
            return Context.GroupHistory
                .Where(h => h.GroupId == groupId)
                .Skip(PageSize * page)
                .Take(PageSize)
                .ToListAsync();
        }
    }
}