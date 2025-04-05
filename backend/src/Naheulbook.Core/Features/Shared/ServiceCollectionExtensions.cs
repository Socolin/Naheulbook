using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Shared;

public static class ServiceCollectionExtensions
{
    public static void AddSharedUtilities(this IServiceCollection services)
    {
        if (services.IsRegistered<IAuthorizationUtil>())
            return;

        services.AddSingleton<IAuthorizationUtil, AuthorizationUtil>();
        services.AddSingleton<IStringCleanupUtil, StringCleanupUtil>();
        services.AddSingleton<IJsonUtil, JsonUtil>();
        services.AddSingleton<ITimeService, TimeService>();
        services.AddSingleton<IRngUtil, RngUtil>();
    }
}