using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Talent;

public static class ServiceCollectionExtensions
{
    public static void AddTalentService(this IServiceCollection services)
    {
        if (services.IsRegistered<ITalentService>())
            return;

        services.AddSingleton<ITalentMapper, TalentMapper>();

        services.AddSingleton<ITalentService, TalentService>();
        services.AddSingleton<ICharacterTalentService, CharacterTalentService>();
    }
}
