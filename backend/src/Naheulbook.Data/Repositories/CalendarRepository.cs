using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface ICalendarRepository : IRepository<CalendarEntity>;

public class CalendarRepository(NaheulbookDbContext context) : Repository<CalendarEntity, NaheulbookDbContext>(context), ICalendarRepository;