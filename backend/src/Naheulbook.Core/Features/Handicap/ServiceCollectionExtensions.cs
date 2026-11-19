using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Handicap;

public static class ServiceCollectionExtensions
{
    public static void AddHandicapService(this IServiceCollection services)
    {
        if (services.IsRegistered<IHandicapService>())
            return;

        services.AddSingleton<IHandicapMapper, HandicapMapper>();

        services.AddSingleton<IHandicapService, HandicapService>();
    }
}
