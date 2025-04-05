using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;

namespace Naheulbook.Core.Features.Calendar;

public interface ICalendarService
{
    Task<List<CalendarEntity>> GetCalendarAsync();
}

public class CalendarService(IUnitOfWorkFactory unitOfWorkFactory) : ICalendarService
{
    public async Task<List<CalendarEntity>> GetCalendarAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Calendar.GetAllAsync();
    }
}