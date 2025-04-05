using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Skill;

public static class ServiceCollectionExtensions
{
    public static void AddSkillService(this IServiceCollection services)
    {
        if (services.IsRegistered<ISkillService>())
            return;

        services.AddSingleton<ISkillService, SkillService>();
    }
}