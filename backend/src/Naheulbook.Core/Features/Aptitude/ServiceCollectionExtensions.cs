using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Aptitude;

public static class ServiceCollectionExtensions
{
    public static void AddAptitudeService(this IServiceCollection services)
    {
        if (services.IsRegistered<IAptitudeGroupService>())
            return;

        services.AddSingleton<IAptitudeGroupService, AptitudeGroupService>();
    }
}