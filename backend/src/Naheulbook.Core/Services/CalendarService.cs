using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

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