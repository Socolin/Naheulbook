#pragma warning disable 8619
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IGroupInviteRepository : IRepository<GroupInviteEntity>
{
    Task<GroupInviteEntity?> GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(int groupId, int characterId);
    Task<List<GroupInviteEntity>> GetInvitesByCharacterIdAsync(int characterId);
}

public class GroupInviteRepository : Repository<GroupInviteEntity, NaheulbookDbContext>, IGroupInviteRepository
{
    public GroupInviteRepository(NaheulbookDbContext context)
        : base(context)
    {
    }

    public Task<GroupInviteEntity?> GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(int groupId, int characterId)
    {
        return Context.GroupInvites
            .Include(x => x.Group)
            .Include(x => x.Character)
            .FirstOrDefaultAsync(x => x.GroupId == groupId && x.CharacterId == characterId);
    }

    public Task<List<GroupInviteEntity>> GetInvitesByCharacterIdAsync(int characterId)
    {
        return Context.GroupInvites
            .Where(x => x.CharacterId == characterId)
            .ToListAsync();
    }
}