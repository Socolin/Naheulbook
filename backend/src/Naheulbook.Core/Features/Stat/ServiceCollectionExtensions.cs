using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Stat;

public static class ServiceCollectionExtensions
{
    public static void AddStatService(this IServiceCollection services)
    {
        if (services.IsRegistered<IStatService>())
            return;

        services.AddSingleton<IStatService, StatService>();
    }
}