using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;

namespace Naheulbook.Core.Services;

public interface ICalendarService
{
    Task<List<CalendarEntity>> GetCalendarAsync();
}

public class CalendarService : ICalendarService
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public CalendarService(IUnitOfWorkFactory unitOfWorkFactory)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    public async Task<List<CalendarEntity>> GetCalendarAsync()
    {
        using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
        {
            return await uow.Calendar.GetAllAsync();
        }
    }
}