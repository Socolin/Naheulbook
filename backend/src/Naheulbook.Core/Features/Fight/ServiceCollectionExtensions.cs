using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Fight;

public static class ServiceCollectionExtensions
{
    public static void AddFightService(this IServiceCollection services)
    {
        if (services.IsRegistered<IFightService>())
            return;

        services.AddSingleton<IFightService, FightService>();
    }
}