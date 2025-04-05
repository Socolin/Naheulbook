using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Loot;

public static class ServiceCollectionExtensions
{
    public static void AddLootService(this IServiceCollection services)
    {
        if (services.IsRegistered<ILootService>())
            return;

        services.AddSingleton<ILootService, LootService>();
    }
}