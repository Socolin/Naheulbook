using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Npc;

public static class ServiceCollectionExtensions
{
    public static void AddNpcService(this IServiceCollection services)
    {
        if (services.IsRegistered<INpcService>())
            return;

        services.AddSingleton<INpcMapper, NpcMapper>();

        services.AddSingleton<INpcService, NpcService>();
    }
}