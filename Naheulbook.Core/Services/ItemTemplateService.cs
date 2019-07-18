using System.Collections.Generic;
using System.Linq;
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
        Task<List<ItemTemplate>> SearchItemTemplateAsync(string filter, int maxResultCount, int? currentUserId);

        Task<ICollection<Slot>> GetItemSlots();
    }

    public class ItemTemplateService : IItemTemplateService
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IAuthorizationUtil _authorizationUtil;
        private readonly IItemTemplateUtil _itemTemplateUtil;
        private readonly IStringCleanupUtil _stringCleanupUtil;
        private readonly IMapper _mapper;

        public ItemTemplateService(
            IUnitOfWorkFactory unitOfWorkFactory,
            IAuthorizationUtil authorizationUtil,
            IMapper mapper,
            IItemTemplateUtil itemTemplateUtil, IStringCleanupUtil stringCleanupUtil
        )
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _authorizationUtil = authorizationUtil;
            _mapper = mapper;
            _itemTemplateUtil = itemTemplateUtil;
            _stringCleanupUtil = stringCleanupUtil;
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
                        await _authorizationUtil.EnsureAdminAccessAsync(executionContext);
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

        public async Task<List<ItemTemplate>> SearchItemTemplateAsync(string filter, int maxResultCount, int? currentUserId)
        {
            var matchingItemTemplates = new List<ItemTemplate>();
            using (var uow = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var cleanFilter = _stringCleanupUtil.RemoveAccents(filter).ToUpperInvariant();

                var exactMatchingItems = await uow.ItemTemplates.GetItemByCleanNameAsync(cleanFilter, maxResultCount, currentUserId, true);
                matchingItemTemplates.AddRange(exactMatchingItems);

                var partialMatchingItems = await uow.ItemTemplates.GetItemByPartialCleanNameAsync(cleanFilter, maxResultCount - matchingItemTemplates.Count, matchingItemTemplates.Select(i => i.Id), currentUserId, true);
                matchingItemTemplates.AddRange(partialMatchingItems);

                var noSeparatorFilter = _stringCleanupUtil.RemoveSeparators(cleanFilter);
                var partialMatchingIgnoreSpacesItems = await uow.ItemTemplates.GetItemByPartialCleanNameWithoutSeparatorAsync(noSeparatorFilter, maxResultCount - matchingItemTemplates.Count, matchingItemTemplates.Select(i => i.Id), currentUserId, true);
                matchingItemTemplates.AddRange(partialMatchingIgnoreSpacesItems);
            }

            return matchingItemTemplates;
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