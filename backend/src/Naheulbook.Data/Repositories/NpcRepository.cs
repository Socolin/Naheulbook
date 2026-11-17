using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface INpcRepository : IRepository<NpcEntity>
{
    Task<List<NpcEntity>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default);
    Task<NpcEntity?> GetWitGroupAsync(int npcId, CancellationToken cancellationToken = default);
}

public class NpcRepository(NaheulbookDbContext context) : Repository<NpcEntity, NaheulbookDbContext>(context), INpcRepository
{
    public Task<List<NpcEntity>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default)
    {
        return Context.Npcs.Where(x => x.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }

    public Task<NpcEntity?> GetWitGroupAsync(int npcId, CancellationToken cancellationToken = default)
    {
        return Context.Npcs
            .Include(x => x.Group)
            .SingleOrDefaultAsync(x => x.Id == npcId, cancellationToken);
    }
}