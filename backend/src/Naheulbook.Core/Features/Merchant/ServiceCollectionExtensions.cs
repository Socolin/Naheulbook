using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Merchant;

public static class ServiceCollectionExtensions
{
    public static void AddMerchantService(this IServiceCollection services)
    {
        if (services.IsRegistered<IMerchantService>())
            return;

        services.AddSingleton<IMerchantService, MerchantService>();

        services.AddSingleton<IMerchantFactory, MerchantFactory>();
    }
}