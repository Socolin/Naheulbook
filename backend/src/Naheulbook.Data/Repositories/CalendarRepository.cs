using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ICalendarRepository : IRepository<CalendarEntity>
{
}

public class CalendarRepository : Repository<CalendarEntity, NaheulbookDbContext>, ICalendarRepository
{
    public CalendarRepository(NaheulbookDbContext context)
        : base(context)
    {
    }
}