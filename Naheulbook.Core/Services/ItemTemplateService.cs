using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IItemTemplateService
    {
        Task<ItemTemplate> CreateItemTemplateAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateRequest request);
        Task<ICollection<Slot>> GetItemSlots();
    }

    public class ItemTemplateService : IItemTemplateService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IMapper _mapper;

        public ItemTemplateService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil,
            IMapper mapper)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
            _mapper = mapper;
        }

        public async Task<ItemTemplate> CreateItemTemplateAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateRequest request)
        {
            if (request.Source == "official")
                await _authorizationUtil.EnsureAdminAccessAsync(executionContext);

            var itemTemplate = _mapper.Map<ItemTemplate>(request);

            if (request.Source != "official")
                itemTemplate.SourceUserId = executionContext.UserId;

            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                uow.ItemTemplates.Add(itemTemplate);
                await uow.CompleteAsync();
            }

            return itemTemplate;
        }

        public async Task<ICollection<Slot>> GetItemSlots()
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                return await uow.Slots.GetAllAsync();
            }
        }
    }
}