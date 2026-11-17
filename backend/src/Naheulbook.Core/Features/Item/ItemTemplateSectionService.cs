using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Item;

public interface IItemTemplateSectionService
{
    Task<IList<ItemTemplateSectionEntity>> GetAllSectionsAsync(CancellationToken cancellationToken = default);
    Task<ItemTemplateSectionEntity> CreateItemTemplateSectionAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSectionRequest request, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> GetItemTemplatesBySectionAsync(int sectionId, int? currentUserId, CancellationToken cancellationToken = default);
}

public class ItemTemplateSectionService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IItemTemplateUtil itemTemplateUtil
) : IItemTemplateSectionService
{
    public async Task<IList<ItemTemplateSectionEntity>> GetAllSectionsAsync(CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.ItemTemplateSections.GetAllWithCategoriesAsync(cancellationToken);
    }

    public async Task<ItemTemplateSectionEntity> CreateItemTemplateSectionAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSectionRequest request, CancellationToken cancellationToken = default)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext, cancellationToken);

        var itemTemplateSection = new ItemTemplateSectionEntity
        {
            Name = request.Name,
            Special = string.Join(",", request.Specials),
            Note = request.Note ?? string.Empty,
            Icon = request.Icon,
            SubCategories = new List<ItemTemplateSubCategoryEntity>(),
        };

        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        uow.ItemTemplateSections.Add(itemTemplateSection);
        await uow.SaveChangesAsync(DoNotInterrupt.Token);

        return itemTemplateSection;
    }

    public async Task<List<ItemTemplateEntity>> GetItemTemplatesBySectionAsync(int sectionId, int? currentUserId, CancellationToken cancellationToken = default)
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        var itemTemplates = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(sectionId, cancellationToken);
        return itemTemplateUtil.FilterItemTemplatesBySource(itemTemplates, currentUserId, true).ToList();
    }
}