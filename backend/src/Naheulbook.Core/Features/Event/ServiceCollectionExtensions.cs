using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Event;

public static class ServiceCollectionExtensions
{
    public static void AddEventService(this IServiceCollection services)
    {
        if (services.IsRegistered<IEventService>())
            return;

        services.AddSingleton<IEventService, EventService>();
    }
}