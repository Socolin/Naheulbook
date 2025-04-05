using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Job;

public static class ServiceCollectionExtensions
{
    public static void AddJobService(this IServiceCollection services)
    {
        if (services.IsRegistered<IJobService>())
            return;

        services.AddSingleton<IJobService, JobService>();
    }
}