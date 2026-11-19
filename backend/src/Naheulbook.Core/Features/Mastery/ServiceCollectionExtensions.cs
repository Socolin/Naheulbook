using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Mastery;

public static class ServiceCollectionExtensions
{
    public static void AddMasteryService(this IServiceCollection services)
    {
        if (services.IsRegistered<IMasteryService>())
            return;

        services.AddSingleton<IMasteryMapper, MasteryMapper>();

        services.AddSingleton<IMasteryService, MasteryService>();
    }
}
