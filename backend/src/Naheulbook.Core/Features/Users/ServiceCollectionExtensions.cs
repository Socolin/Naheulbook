using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Users;

public static class ServiceCollectionExtensions
{
    public static void AddUserService(this IServiceCollection services)
    {
        if (services.IsRegistered<IUserService>())
            return;

        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IMailService, MailService>();
        services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<ISocialMediaUserLinkService, SocialMediaUserLinkService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<IUserAccessTokenService, UserAccessTokenService>();

        services.AddOptions<MailOptions>()
            .BindConfiguration("Mail")
            .ValidateOnStart()
            .ValidateDataAnnotations();
    }
}