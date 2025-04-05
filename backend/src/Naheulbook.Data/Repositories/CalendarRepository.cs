using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.Data.Repositories;

public interface ICalendarRepository : IRepository<CalendarEntity>;

public class CalendarRepository(NaheulbookDbContext context) : Repository<CalendarEntity, NaheulbookDbContext>(context), ICalendarRepository;