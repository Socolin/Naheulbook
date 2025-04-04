using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Item;

public interface IItemTemplateSubCategoryService
{
    Task<ItemTemplateSubCategoryEntity> CreateItemTemplateSubCategoryAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSubCategoryRequest request);
    Task<List<ItemTemplateEntity>> GetItemTemplatesBySubCategoryTechNameAsync(string subCategoryTechName, int? currentUserId, bool includeCommunityItems);
}

public class ItemTemplateSubCategoryService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil
) : IItemTemplateSubCategoryService
{
    public async Task<ItemTemplateSubCategoryEntity> CreateItemTemplateSubCategoryAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSubCategoryRequest request)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

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
        await uow.SaveChangesAsync();

        return itemTemplateSubCategory;
    }

    public async Task<List<ItemTemplateEntity>> GetItemTemplatesBySubCategoryTechNameAsync(string subCategoryTechName, int? currentUserId, bool includeCommunityItems)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var itemTemplateSubCategory = await uow.ItemTemplateSubCategories.GetByTechNameAsync(subCategoryTechName);
            if (itemTemplateSubCategory == null)
                throw new ItemTemplateSubCategoryNotFoundException(subCategoryTechName);

            return await uow.ItemTemplates.GetWithAllDataByCategoryIdAsync(itemTemplateSubCategory.Id, currentUserId, includeCommunityItems);
        }
    }
}