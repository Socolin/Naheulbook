using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.God;

public static class ServiceCollectionExtensions
{
    public static void AddGodService(this IServiceCollection services)
    {
        if (services.IsRegistered<IGodService>())
            return;

        services.AddSingleton<IGodService, GodService>();
    }
}