using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Shared.Clients.MicrosoftGraph;

public static class ServiceCollectionExtensions
{
    public static void AddMicrosoftGraphClient(this IServiceCollection services)
    {
        if (services.IsRegistered<IMicrosoftGraphClient>())
            return;

        services.AddSingleton<IMicrosoftGraphClient, MicrosoftGraphClient>();

        services.AddOptions<MicrosoftGraphOptions>()
            .BindConfiguration("Authentication:MicrosoftGraph")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}