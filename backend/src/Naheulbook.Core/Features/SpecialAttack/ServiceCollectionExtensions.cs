using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.SpecialAttack;

public static class ServiceCollectionExtensions
{
    public static void AddSpecialAttackService(this IServiceCollection services)
    {
        if (services.IsRegistered<ISpecialAttackService>())
            return;

        services.AddSingleton<ISpecialAttackMapper, SpecialAttackMapper>();

        services.AddSingleton<ISpecialAttackService, SpecialAttackService>();
    }
}
