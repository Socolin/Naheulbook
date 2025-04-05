using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface IEventRepository : IRepository<EventEntity>
{
    Task<List<EventEntity>> GetByGroupIdAsync(int groupId);
}

public class EventRepository(NaheulbookDbContext context) : Repository<EventEntity, NaheulbookDbContext>(context), IEventRepository
{
    public Task<List<EventEntity>> GetByGroupIdAsync(int groupId)
    {
        return Context.Events
            .Where(g => g.GroupId == groupId)
            .ToListAsync();
    }
}