using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface INpcRepository : IRepository<NpcEntity>
{
    Task<List<NpcEntity>> GetByGroupIdAsync(int groupId);
    Task<NpcEntity?> GetWitGroupAsync(int npcId);
}

public class NpcRepository(NaheulbookDbContext context) : Repository<NpcEntity, NaheulbookDbContext>(context), INpcRepository
{
    public Task<List<NpcEntity>> GetByGroupIdAsync(int groupId)
    {
        return Context.Npcs.Where(x => x.GroupId == groupId)
            .ToListAsync();
    }

    public Task<NpcEntity?> GetWitGroupAsync(int npcId)
    {
        return Context.Npcs
            .Include(x => x.Group)
            .SingleOrDefaultAsync(x => x.Id == npcId);
    }
}