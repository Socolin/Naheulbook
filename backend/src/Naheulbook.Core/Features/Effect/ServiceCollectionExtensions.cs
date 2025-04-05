using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Effect;

public static class ServiceCollectionExtensions
{
    public static void AddEffectService(this IServiceCollection services)
    {
        if (services.IsRegistered<IEffectService>())
            return;

        services.AddSingleton<IEffectService, EffectService>();
    }
}