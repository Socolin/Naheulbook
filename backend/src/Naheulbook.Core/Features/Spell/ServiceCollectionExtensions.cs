using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Spell;

public static class ServiceCollectionExtensions
{
    public static void AddSpellService(this IServiceCollection services)
    {
        if (services.IsRegistered<ISpellService>())
            return;

        services.AddSingleton<ISpellMapper, SpellMapper>();

        services.AddSingleton<ISpellService, SpellService>();
    }
}
