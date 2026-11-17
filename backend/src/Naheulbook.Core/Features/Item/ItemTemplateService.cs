using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Item;

public interface IItemTemplateService
{
    Task<ItemTemplateEntity> GetItemTemplateAsync(Guid itemTemplateId, CancellationToken cancellationToken = default);
    Task<ItemTemplateEntity> CreateItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplateRequest request, CancellationToken cancellationToken = default);
    Task<ItemTemplateEntity> EditItemTemplateAsync(NaheulbookExecutionContext executionContext, Guid itemTemplateId, ItemTemplateRequest request, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> SearchItemTemplateAsync(string filter, int maxResultCount, int? currentUserId, CancellationToken cancellationToken = default);

    Task<ICollection<SlotEntity>> GetItemSlotsAsync(CancellationToken cancellationToken = default);
}

public class ItemTemplateService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IItemTemplateMapper itemTemplateMapper,
    IItemTemplateUtil itemTemplateUtil,
    IStringCleanupUtil stringCleanupUtil
) : IItemTemplateService
{
    public async Task<ItemTemplateEntity> GetItemTemplateAsync(Guid itemTemplateId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var itemTemplate = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId, cancellationToken);
        if (itemTemplate == null)
            throw new ItemTemplateNotFoundException(itemTemplateId);
        return itemTemplate;
    }

    public async Task<ItemTemplateEntity> CreateItemTemplateAsync(NaheulbookExecutionContext executionContext, ItemTemplateRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Source == "official")
            await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        var itemTemplate = itemTemplateMapper.ToEntity(request);

        if (request.Source != "official")
            itemTemplate.SourceUserId = executionContext.UserId;

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        uow.ItemTemplates.Add(itemTemplate);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);
        itemTemplate = (await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplate.Id, cancellationToken))!;

        return itemTemplate;
    }

    public async Task<ItemTemplateEntity> EditItemTemplateAsync(NaheulbookExecutionContext executionContext, Guid itemTemplateId, ItemTemplateRequest request, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var itemTemplate = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId, cancellationToken);
        if (itemTemplate == null)
            throw new ItemTemplateNotFoundException(itemTemplateId);

        await authorizationUtil.EnsureCanEditItemTemplateAsync(executionContext, itemTemplate, cancellationToken);

        if (itemTemplate.Source != request.Source)
        {
            if (request.Source == "official")
            {
                await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);
                itemTemplate.SourceUserId = null;
            }
            else
                itemTemplate.SourceUserId = executionContext.UserId;
        }

        itemTemplateUtil.ApplyChangesFromRequest(itemTemplate, request);

        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return (await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplateId, cancellationToken))!;
    }

    public async Task<List<ItemTemplateEntity>> SearchItemTemplateAsync(string filter, int maxResultCount, int? currentUserId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filter))
            return new List<ItemTemplateEntity>();

        var matchingItemTemplates = new List<ItemTemplateEntity>();
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var cleanFilter = stringCleanupUtil.RemoveAccents(filter).ToUpperInvariant();

        var exactMatchingItems = await uow.ItemTemplates.GetItemByCleanNameWithAllDataAsync(cleanFilter, maxResultCount, currentUserId, true, cancellationToken);
        matchingItemTemplates.AddRange(exactMatchingItems);

        var partialMatchingItems = await uow.ItemTemplates.GetItemByPartialCleanNameWithAllDataAsync(cleanFilter, maxResultCount - matchingItemTemplates.Count, matchingItemTemplates.Select(i => i.Id), currentUserId, true, cancellationToken);
        matchingItemTemplates.AddRange(partialMatchingItems);

        var noSeparatorFilter = stringCleanupUtil.RemoveSeparators(cleanFilter);
        var partialMatchingIgnoreSpacesItems = await uow.ItemTemplates.GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(noSeparatorFilter, maxResultCount - matchingItemTemplates.Count, matchingItemTemplates.Select(i => i.Id), currentUserId, true, cancellationToken);
        matchingItemTemplates.AddRange(partialMatchingIgnoreSpacesItems);

        return matchingItemTemplates;
    }

    public async Task<ICollection<SlotEntity>> GetItemSlotsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.Slots.GetAllAsync(cancellationToken);
    }
}