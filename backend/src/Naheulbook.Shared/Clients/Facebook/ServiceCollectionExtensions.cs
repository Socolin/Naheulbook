using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Shared.Clients.Facebook;

public static class ServiceCollectionExtensions
{
    public static void AddFacebookClient(this IServiceCollection services)
    {
        if (services.IsRegistered<IFacebookClient>())
            return;

        services.AddSingleton<IFacebookClient, FacebookClient>();

        services.AddOptions<FacebookOptions>()
            .BindConfiguration("Authentication:Facebook")
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}