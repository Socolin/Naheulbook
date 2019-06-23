using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories
{
    public interface ICalendarRepository : IRepository<Calendar>
    {
    }

    public class CalendarRepository : Repository<Calendar, NaheulbookDbContext>, ICalendarRepository
    {
        public CalendarRepository(NaheulbookDbContext context)
            : base(context)
        {
        }
    }
}