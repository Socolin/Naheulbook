using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Monster;

public static class ServiceCollectionExtensions
{
    public static void AddMonsterService(this IServiceCollection services)
    {
        if (services.IsRegistered<IMonsterService>())
            return;

        services.AddSingleton<IMonsterService, MonsterService>();
        services.AddSingleton<IMonsterTemplateService, MonsterTemplateService>();
        services.AddSingleton<IMonsterTraitService, MonsterTraitService>();
        services.AddSingleton<IMonsterTypeService, MonsterTypeService>();

        services.AddSingleton<IActiveStatsModifierUtil, ActiveStatsModifierUtil>();
    }
}