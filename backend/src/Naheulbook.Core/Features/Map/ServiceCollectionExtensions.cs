using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Map;

public static class ServiceCollectionExtensions
{
    public static void AddMapService(this IServiceCollection services)
    {
        if (services.IsRegistered<IMapService>())
            return;

        services.AddSingleton<IMapService, MapService>();

        services.AddSingleton<IMapImageUtil, MapImageUtil>();

        services.AddOptions<MapImageOptions>()
            .BindConfiguration("MapImage")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}