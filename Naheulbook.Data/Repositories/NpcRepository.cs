using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface INpcRepository : IRepository<Npc>
    {
        Task<List<Npc>> GetByGroupIdAsync(int groupId);
        Task<Npc> GetWitGroupAsync(int npcId);
    }

    public class NpcRepository : Repository<Npc, NaheulbookDbContext>, INpcRepository
    {
        public NpcRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<Npc>> GetByGroupIdAsync(int groupId)
        {
            return Context.Npcs.Where(x => x.GroupId == groupId)
                .ToListAsync();
        }

        public Task<Npc> GetWitGroupAsync(int npcId)
        {
            return Context.Npcs
                .Include(x => x.Group)
                .SingleOrDefaultAsync(x => x.Id == npcId);
        }
    }
}