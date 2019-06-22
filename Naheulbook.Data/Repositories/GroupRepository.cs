using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<List<Group>> GetGroupsOwnedByAsync(int userId);
    }

    public class GroupRepository : Repository<Group, NaheulbookDbContext>, IGroupRepository
    {
        public GroupRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<Group>> GetGroupsOwnedByAsync(int userId)
        {
            return Context.Groups
                .Include(g => g.Characters)
                .Where(g => g.MasterId == userId)
                .ToListAsync();
        }
    }
}