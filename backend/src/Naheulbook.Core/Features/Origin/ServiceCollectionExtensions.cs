using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Origin;

public static class ServiceCollectionExtensions
{
    public static void AddOriginService(this IServiceCollection services)
    {
        if (services.IsRegistered<IOriginService>())
            return;

        services.AddSingleton<IOriginService, OriginService>();

        services.AddSingleton<IOriginUtil, OriginUtil>();
    }
}