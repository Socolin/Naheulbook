using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IGroupInviteRepository : IRepository<GroupInvite>
    {
        Task<GroupInvite> GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(int groupId, int characterId);
    }

    public class GroupInviteRepository : Repository<GroupInvite, NaheulbookDbContext>, IGroupInviteRepository
    {
        public GroupInviteRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<GroupInvite> GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(int groupId, int characterId)
        {
            return Context.GroupInvites
                .Include(x => x.Group)
                .Include(x => x.Character)
                .FirstOrDefaultAsync(x => x.GroupId == groupId && x.CharacterId == characterId);
        }
    }
}