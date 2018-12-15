using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IItemTemplateCategoryService
    {
        Task<ItemTemplateCategory> CreateItemTemplateCategoryAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateCategoryRequest request);
    }

    public class ItemTemplateCategoryService : IItemTemplateCategoryService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;

        public ItemTemplateCategoryService(IUnitOfWorkFactory unitOfWorkFactory, IAuthorizationUtil authorizationUtil)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
        }

        public async Task<ItemTemplateCategory> CreateItemTemplateCategoryAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateCategoryRequest request)
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            var itemTemplateCategory = new ItemTemplateCategory()
            {
                SectionId = request.SectionId,
                Name = request.Name,
                Note = request.Note,
                Description = request.Description,
                TechName = request.TechName
            };

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                uow.ItemTemplateCategories.Add(itemTemplateCategory);
                await uow.CompleteAsync();
            }

            return itemTemplateCategory;
        }
    }
}