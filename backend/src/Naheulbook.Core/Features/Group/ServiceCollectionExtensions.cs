using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Group;

public static class ServiceCollectionExtensions
{
    public static void AddGroupService(this IServiceCollection services)
    {
        if (services.IsRegistered<IGroupService>())
            return;

        services.AddSingleton<IGroupService, GroupService>();

        services.AddSingleton<IDurationUtil, DurationUtil>();
        services.AddSingleton<IGroupConfigUtil, GroupConfigUtil>();
        services.AddSingleton<IGroupHistoryUtil, GroupHistoryUtil>();
        services.AddSingleton<IGroupUtil, GroupUtil>();
    }
}