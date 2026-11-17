#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IGroupInviteRepository : IRepository<GroupInviteEntity>
{
    Task<GroupInviteEntity?> GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(int groupId, int characterId, CancellationToken cancellationToken = default);
    Task<List<GroupInviteEntity>> GetInvitesByCharacterIdAsync(int characterId, CancellationToken cancellationToken = default);
}

public class GroupInviteRepository(NaheulbookDbContext context) : Repository<GroupInviteEntity, NaheulbookDbContext>(context), IGroupInviteRepository
{
    public Task<GroupInviteEntity?> GetByCharacterIdAndGroupIdWithGroupWithCharacterAsync(int groupId, int characterId, CancellationToken cancellationToken = default)
    {
        return Context.GroupInvites
            .Include(x => x.Group)
            .Include(x => x.Character)
            .FirstOrDefaultAsync(x => x.GroupId == groupId && x.CharacterId == characterId, cancellationToken);
    }

    public Task<List<GroupInviteEntity>> GetInvitesByCharacterIdAsync(int characterId, CancellationToken cancellationToken = default)
    {
        return Context.GroupInvites
            .Where(x => x.CharacterId == characterId)
            .ToListAsync(cancellationToken);
    }
}