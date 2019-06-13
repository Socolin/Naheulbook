using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Factories;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Services
{
    public interface IItemTemplateService
    {
        Task<ItemTemplate> GetItemTemplateAsync(int itemTemplateId);
        Task<ItemTemplate> CreateItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplateRequest request);
        Task<ItemTemplate> EditItemTemplateAsync(NaheulbookExecutionContext executionContext, int itemTemplateId, ItemTemplateRequest request);

        Task<ICollection<Slot>> GetItemSlots();
    }

    public class ItemTemplateService : IItemTemplateService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IItemTemplateUtil _itemTemplateUtil;
        private readonly IMapper _mapper;

        public ItemTemplateService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil,
            IMapper mapper,
            IItemTemplateUtil itemTemplateUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
            _mapper = mapper;
            _itemTemplateUtil = itemTemplateUtil;
        }

        public async Task<ItemTemplate> GetItemTemplateAsync(int itemTemplateId)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var itemTemplate = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId);
                if (itemTemplate == null)
                    throw new ItemTemplateNotFoundException(itemTemplateId);
                return itemTemplate;
            }
        }

        public async Task<ItemTemplate> CreateItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplateRequest request)
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
                itemTemplate = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplate.Id);
            }

            return itemTemplate;
        }

        public async Task<ItemTemplate> EditItemTemplateAsync(NaheulbookExecutionContext executionContext, int itemTemplateId, ItemTemplateRequest request)
        {
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var itemTemplate = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId);
                if (itemTemplate == null)
                    throw new ItemTemplateNotFoundException(itemTemplateId);

                await _authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate);

                if (itemTemplate.Source != request.Source)
                {
                    if (request.Source == "official")
                    {
                        await _authorizationUtil.EnsureAdminAccessAsync(executionContext); // TODO: Test this
                        itemTemplate.SourceUserId = null;
                    }
                    else
                        itemTemplate.SourceUserId = executionContext.UserId;
                }

                _itemTemplateUtil.ApplyChangesFromRequest(itemTemplate, request);

                await uow.CompleteAsync();

                return await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId);
            }
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