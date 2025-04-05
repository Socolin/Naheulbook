using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.UnitOfWorks;
using Naheulbook.Requests.Requests;

namespace Naheulbook.Core.Features.Item;

public interface IItemTemplateSectionService
{
    Task<IList<ItemTemplateSectionEntity>> GetAllSectionsAsync();
    Task<ItemTemplateSectionEntity> CreateItemTemplateSectionAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSectionRequest request);
    Task<List<ItemTemplateEntity>> GetItemTemplatesBySectionAsync(int sectionId, int? currentUserId);
}

public class ItemTemplateSectionService(
    IUnitOfWorkFactory unitOfWorkFactory,
    IAuthorizationUtil authorizationUtil,
    IItemTemplateUtil itemTemplateUtil
) : IItemTemplateSectionService
{
    public async Task<IList<ItemTemplateSectionEntity>> GetAllSectionsAsync()
    {
        using var uow = unitOfWorkFactory.CreateUnitOfWork();
        return await uow.ItemTemplateSections.GetAllWithCategoriesAsync();
    }

    public async Task<ItemTemplateSectionEntity> CreateItemTemplateSectionAsync(NaheulbookExecutionContext executionContext, CreateItemTemplateSectionRequest request)
    {
        await authorizationUtil.EnsureAdminAccessAsync(executionContext);

        var itemTemplateSection = new ItemTemplateSectionEntity
        {
            Name = request.Name,
            Special = string.Join(",", request.Specials),
            Note = request.Note ?? string.Empty,
            Icon = request.Icon,
            SubCategories = new List<ItemTemplateSubCategoryEntity>(),
        };

        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            uow.ItemTemplateSections.Add(itemTemplateSection);
            await uow.SaveChangesAsync();
        }

        return itemTemplateSection;
    }

    public async Task<List<ItemTemplateEntity>> GetItemTemplatesBySectionAsync(int sectionId, int? currentUserId)
    {
        using (var uow = unitOfWorkFactory.CreateUnitOfWork())
        {
            var itemTemplates = await uow.ItemTemplates.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(sectionId);
            return itemTemplateUtil.FilterItemTemplatesBySource(itemTemplates, currentUserId, true).ToList();
        }
    }
}