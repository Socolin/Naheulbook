using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Entropy;

public static class ServiceCollectionExtensions
{
    public static void AddEntropyService(this IServiceCollection services)
    {
        if (services.IsRegistered<IEntropyService>())
            return;

        services.AddSingleton<IEntropyMapper, EntropyMapper>();
        services.AddSingleton<IEntropyService, EntropyService>();
    }
}
