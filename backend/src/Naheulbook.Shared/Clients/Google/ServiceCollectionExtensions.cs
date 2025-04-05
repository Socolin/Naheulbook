using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Shared.Clients.Google;

public static class ServiceCollectionExtensions
{
    public static void AddGoogleClient(this IServiceCollection services)
    {
        if (services.IsRegistered<IGoogleClient>())
            return;

        services.AddSingleton<IGoogleClient, GoogleClient>();

        services.AddOptions<GoogleOptions>()
            .BindConfiguration("Authentication:Google")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}