using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Core.Features.Character;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Aptitude;

public static class ServiceCollectionExtensions
{
    public static void AddAptitudeService(this IServiceCollection services)
    {
        if (services.IsRegistered<IAptitudeGroupService>())
            return;

        services.AddSingleton<IAptitudeGroupService, AptitudeGroupService>();
        services.AddSingleton<ICharacterAptitudeFactory, CharacterAptitudeFactory>();
        services.AddSingleton<ICharacterAptitudeService, CharacterAptitudeService>();
    }
}