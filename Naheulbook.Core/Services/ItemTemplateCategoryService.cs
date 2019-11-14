using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IItemTemplateSubCategoryService
    {
        Task<ItemTemplateSubCategory> CreateItemTemplateSubCategoryAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSubCategoryRequest request);
        Task<List<ItemTemplate>> GetItemTemplatesBySubCategoryTechNameAsync(string subCategoryTechName, int? currentUserId, bool includeCommunityItems);
    }

    public class ItemTemplateSubCategoryService : IItemTemplateSubCategoryService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;

        public ItemTemplateSubCategoryService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
        }

        public async Task<ItemTemplateSubCategory> CreateItemTemplateSubCategoryAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSubCategoryRequest request)
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            var itemTemplateSubCategory = new ItemTemplateSubCategory()
            {
                SectionId = request.SectionId,
                Name = request.Name,
                Note = request.Note,
                Description = request.Description,
                TechName = request.TechName
            };

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                uow.ItemTemplateSubCategories.Add(itemTemplateSubCategory);
                await uow.SaveChangesAsync();
            }

            return itemTemplateSubCategory;
        }

        public async Task<List<ItemTemplate>> GetItemTemplatesBySubCategoryTechNameAsync(string subCategoryTechName, int? currentUserId, bool includeCommunityItems)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var itemTemplateSubCategory = await uow.ItemTemplateSubCategories.GetByTechNameAsync(subCategoryTechName);
                if (itemTemplateSubCategory == null)
                    throw new ItemTemplateSubCategoryNotFoundException(subCategoryTechName);

                return await uow.ItemTemplates.GetWithAllDataByCategoryIdAsync(itemTemplateSubCategory.Id, currentUserId, includeCommunityItems);
            }
        }
    }
}