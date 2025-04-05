using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Calendar;

public static class ServiceCollectionExtensions
{
    public static void AddCalendarService(this IServiceCollection services)
    {
        if (services.IsRegistered<ICalendarService>())
            return;

        services.AddSingleton<ICalendarService, CalendarService>();
    }
}