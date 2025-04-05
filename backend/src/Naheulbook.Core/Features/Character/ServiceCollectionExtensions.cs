using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Character;

public static class ServiceCollectionExtensions
{
    public static void AddCharacterService(this IServiceCollection services)
    {
        if (services.IsRegistered<ICharacterService>())
            return;

        services.AddSingleton<ICharacterBackupService, CharacterBackupService>();
        services.AddSingleton<ICharacterRandomNameService, CharacterRandomNameService>();
        services.AddSingleton<ICharacterService, CharacterService>();

        services.AddSingleton<ICharacterHistoryUtil, CharacterHistoryUtil>();
        services.AddSingleton<ICharacterModifierUtil, CharacterModifierUtil>();
        services.AddSingleton<ICharacterUtil, CharacterUtil>();
        services.AddSingleton<ICharacterFactory, CharacterFactory>();

        services.AddOptions<LaPageAMelkorClient.Options>()
            .BindConfiguration("LaPageAMelkor")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddHttpClient<ILaPageAMelkorClient, LaPageAMelkorClient>((sp, client) =>
            {
                client.BaseAddress = new Uri(sp.GetRequiredService<IOptions<LaPageAMelkorClient.Options>>().Value.Url);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "Naheulbook");
            }
        );
    }
}