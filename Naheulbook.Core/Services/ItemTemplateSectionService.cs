using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IItemTemplateSectionService
    {
        Task<IList<ItemTemplateSection>> GetAllSections();
        Task<ItemTemplateSection> CreateItemTemplateSectionAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSectionRequest request);
    }

    public class ItemTemplateSectionService : IItemTemplateSectionService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;

        public ItemTemplateSectionService(IUnitOfWorkFactory unitOfWorkFactory, IAuthorizationUtil authorizationUtil)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
        }

        public async Task<IList<ItemTemplateSection>> GetAllSections()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.ItemTemplateSections.GetAllWithCategoriesAsync();
            }
        }

        public async Task<ItemTemplateSection> CreateItemTemplateSectionAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSectionRequest request)
        {
            await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            var itemTemplateSection = new ItemTemplateSection()
            {
                Name = request.Name,
                Special = request.Specials == null ? null : string.Join(",", request.Specials),
                Note = request.Note
            };

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                uow.ItemTemplateSections.Add(itemTemplateSection);
                await uow.CompleteAsync();
            }

            return itemTemplateSection;
        }
    }
}