using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Shared.Clients.Twitter;

public static class ServiceCollectionExtensions
{
    public static void AddTwitterClient(this IServiceCollection services)
    {
        if (services.IsRegistered<ITwitterClient>())
            return;

        services.AddSingleton<ITwitterClient, TwitterClient>();

        services.AddOptions<TwitterOptions>()
            .BindConfiguration("Authentication:Twitter")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}