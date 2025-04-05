using Microsoft.Extensions.DependencyInjection;
using Naheulbook.Core.Features.Item.Actions.Executor;
using Naheulbook.Shared.Extensions;

namespace Naheulbook.Core.Features.Item;

public static class ServiceCollectionExtensions
{
    public static void AddItemService(this IServiceCollection services)
    {
        if (services.IsRegistered<IItemService>())
            return;

        services.AddSingleton<IItemService, ItemService>();
        services.AddSingleton<IItemTemplateSectionService, ItemTemplateSectionService>();
        services.AddSingleton<IItemTemplateService, ItemTemplateService>();
        services.AddSingleton<IItemTemplateSubCategoryService, ItemTemplateSubCategoryService>();
        services.AddSingleton<IItemTypeService, ItemTypeService>();

        services.AddSingleton<IActionsUtil, ActionsUtil>();
        services.AddSingleton<IItemDataUtil, ItemDataUtil>();
        services.AddSingleton<IItemFactory, ItemFactory>();
        services.AddSingleton<IItemTemplateUtil, ItemTemplateUtil>();
        services.AddSingleton<IItemUtil, ItemUtil>();

        services.AddSingleton<IAddCustomModifierExecutor, AddCustomModifierExecutor>();
        services.AddSingleton<IAddEaExecutor, AddEaExecutor>();
        services.AddSingleton<IAddEffectExecutor, AddEffectExecutor>();
        services.AddSingleton<IAddEvExecutor, AddEvExecutor>();
        services.AddSingleton<IAddItemExecutor, AddItemExecutor>();
        services.AddSingleton<IRemoveItemExecutor, RemoveItemExecutor>();
    }
}