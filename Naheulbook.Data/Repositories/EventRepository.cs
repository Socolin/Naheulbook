using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<List<Event>> GetByGroupIdAsync(int groupId);
    }

    public class EventRepository : Repository<Event, NaheulbookDbContext>, IEventRepository
    {
        public EventRepository(NaheulbookDbContext context)
            : base(context)
        {
        }

        public Task<List<Event>> GetByGroupIdAsync(int groupId)
        {
            return Context.Events
                .Where(g => g.GroupId == groupId)
                .ToListAsync();
        }
    }
}