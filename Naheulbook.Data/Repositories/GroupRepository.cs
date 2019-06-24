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
        Task<Group> GetGroupsWithDetailsAsync(int groupId);
        Task<Group> GetGroupsWithCharactersAsync(int groupId);
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

        public Task<Group> GetGroupsWithDetailsAsync(int groupId)
        {
            return Context.Groups
                .Include(g => g.Characters)
                .Include(g => g.Invites)
                .Include(g => g.Location)
                .Where(g => g.Id == groupId)
                .SingleOrDefaultAsync();
        }

        public Task<Group> GetGroupsWithCharactersAsync(int groupId)
        {
            return Context.Groups
                .Include(g => g.Characters)
                .Where(g => g.Id == groupId)
                .SingleOrDefaultAsync();
        }
    }
}