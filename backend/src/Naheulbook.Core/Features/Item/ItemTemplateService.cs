using AutoMapper;
using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Item;

public interface IItemTemplateService
{
    Task<ItemTemplateEntity> GetItemTemplateAsync(Guid itemTemplateId);
    Task<ItemTemplateEntity> CreateItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplateRequest request);
    Task<ItemTemplateEntity> EditItemTemplateAsync(NaheulbookExecutionContext executionContext, Guid itemTemplateId, ItemTemplateRequest request);
    Task<List<ItemTemplateEntity>> SearchItemTemplateAsync(string filter, int maxResultCount, int? currentUserId);

    Task<ICollection<SlotEntity>> GetItemSlotsAsync();
}

public class ItemTemplateService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IMapper mapper,
    IItemTemplateUtil itemTemplateUtil,
    IStringCleanupUtil stringCleanupUtil
) : IItemTemplateService
{
    public async Task<ItemTemplateEntity> GetItemTemplateAsync(Guid itemTemplateId)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var itemTemplate = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId);
        if (itemTemplate == null)
            throw new ItemTemplateNotFoundException(itemTemplateId);
        return itemTemplate;
    }

    public async Task<ItemTemplateEntity> CreateItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplateRequest request)
    {
        if (request.Source == "official")
            await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        var itemTemplate = mapper.Map<ItemTemplateEntity>(request);

        if (request.Source != "official")
            itemTemplate.SourceUserId = executionContext.UserId;

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        uow.ItemTemplates.Add(itemTemplate);
        await uow.SaveChangesAsync();
        itemTemplate = (await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplate.Id))!;

        return itemTemplate;
    }

    public async Task<ItemTemplateEntity> EditItemTemplateAsync(NaheulbookExecutionContext executionContext, Guid itemTemplateId, ItemTemplateRequest request)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var itemTemplate = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId);
        if (itemTemplate == null)
            throw new ItemTemplateNotFoundException(itemTemplateId);

        await authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate);

        if (itemTemplate.Source != request.Source)
        {
            if (request.Source == "official")
            {
                await authorizationUtil.EnsureAdminAccessAsync(executionContext);
                itemTemplate.SourceUserId = null;
            }
            else
                itemTemplate.SourceUserId = executionContext.UserId;
        }

        itemTemplateUtil.ApplyChangesFromRequest(itemTemplate, request);

        await uow.SaveChangesAsync();

        return (await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId))!;
    }

    public async Task<List<ItemTemplateEntity>> SearchItemTemplateAsync(string filter, int maxResultCount, int? currentUserId)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return new List<ItemTemplateEntity>();

        var matchingItemTemplates = new List<ItemTemplateEntity>();
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var cleanFilter = stringCleanupUtil.RemoveAccents(filter).ToUpperInvariant();

        var exactMatchingItems = await uow.ItemTemplates.GetItemByCleanNameWithAllDataAsync(cleanFilter, maxResultCount, currentUserId, true);
        matchingItemTemplates.AddRange(exactMatchingItems);

        var partialMatchingItems = await uow.ItemTemplates.GetItemByPartialCleanNameWithAllDataAsync(cleanFilter, maxResultCount - matchingItemTemplates.Count, matchingItemTemplates.Select(i => i.Id), currentUserId, true);
        matchingItemTemplates.AddRange(partialMatchingItems);

        var noSeparatorFilter = stringCleanupUtil.RemoveSeparators(cleanFilter);
        var partialMatchingIgnoreSpacesItems = await uow.ItemTemplates.GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(noSeparatorFilter, maxResultCount - matchingItemTemplates.Count, matchingItemTemplates.Select(i => i.Id), currentUserId, true);
        matchingItemTemplates.AddRange(partialMatchingIgnoreSpacesItems);

        return matchingItemTemplates;
    }

    public async Task<ICollection<SlotEntity>> GetItemSlotsAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Slots.GetAllAsync();
    }
}