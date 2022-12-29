using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IEventRepository : IRepository<EventEntity>
{
    Task<List<EventEntity>> GetByGroupIdAsync(int groupId);
}

public class EventRepository : Repository<EventEntity, NaheulbookDbContext>, IEventRepository
{
    public EventRepository(NaheulbookDbContext context)
        : base(context)
    {
    }

    public Task<List<EventEntity>> GetByGroupIdAsync(int groupId)
    {
        return Context.Events
            .Where(g => g.GroupId == groupId)
            .ToListAsync();
    }
}