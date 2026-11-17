using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Item;

public interface IItemTemplateSubCategoryService
{
    Task<ItemTemplateSubCategoryEntity> CreateItemTemplateSubCategoryAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSubCategoryRequest request, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> GetItemTemplatesBySubCategoryTechNameAsync(string subCategoryTechName, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default);
}

public class ItemTemplateSubCategoryService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil
) : IItemTemplateSubCategoryService
{
    public async Task<ItemTemplateSubCategoryEntity> CreateItemTemplateSubCategoryAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSubCategoryRequest request, CancellationToken cancellationToken = default)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        var itemTemplateSubCategory = new ItemTemplateSubCategoryEntity()
        {
            SectionId = request.SectionId,
            Name = request.Name,
            Note = request.Note ?? string.Empty,
            Description = request.Description ?? string.Empty,
            TechName = request.TechName ?? string.Empty,
        };

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        uow.ItemTemplateSubCategories.Add(itemTemplateSubCategory);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return itemTemplateSubCategory;
    }

    public async Task<List<ItemTemplateEntity>> GetItemTemplatesBySubCategoryTechNameAsync(string subCategoryTechName, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var itemTemplateSubCategory = await uow.ItemTemplateSubCategories.GetByTechNameAsync(subCategoryTechName, cancellationToken);
        if (itemTemplateSubCategory == null)
            throw new ItemTemplateSubCategoryNotFoundException(subCategoryTechName);

        return await uow.ItemTemplates.GetWithAllDataByCategoryIdAsync(itemTemplateSubCategory.Id, currentUserId, includeCommunityItems, cancellationToken);
    }
}