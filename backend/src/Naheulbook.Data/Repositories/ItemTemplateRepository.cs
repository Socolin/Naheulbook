#pragma warning disable 8619
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Extensions;

namespace Naheulbook.Data.Repositories;

public interface IItemTemplateRepository : IRepository<ItemTemplateEntity>
{
    Task<ItemTemplateEntity?> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(int sectionId, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> GetWithAllDataByCategoryIdAsync(int subCategoryId, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> GetItemByCleanNameWithAllDataAsync(string name, int maxResultCount, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> GetItemByPartialCleanNameWithAllDataAsync(string name, int maxResultCount, IEnumerable<Guid> excludedIds, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default);
    Task<List<ItemTemplateEntity>> GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(string name, int maxResultCount, IEnumerable<Guid> excludedIds, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default);
    Task<ItemTemplateEntity> GetPurseItemTemplateBasedOnMoneyAsync(int money, CancellationToken cancellationToken = default);
    Task<ItemTemplateEntity> GetGoldCoinItemTemplate(CancellationToken cancellationToken = default);
}

public class ItemTemplateRepository(NaheulbookDbContext context) : Repository<ItemTemplateEntity, NaheulbookDbContext>(context), IItemTemplateRepository
{
    public Task<ItemTemplateEntity?> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplates
            .IncludeItemTemplateDetails()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<List<ItemTemplateEntity>> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplates
            .IncludeItemTemplateDetails()
            .Where(x => x.SubCategory.SectionId == sectionId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ItemTemplateEntity>> GetWithAllDataByCategoryIdAsync(int subCategoryId, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplates
            .IncludeItemTemplateDetails()
            .FilterCommunityAndPrivateItemTemplates(currentUserId, includeCommunityItems)
            .Where(x => x.SubCategoryId == subCategoryId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ItemTemplateEntity>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplates
            .IncludeItemTemplateDetails()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<List<ItemTemplateEntity>> GetItemByCleanNameWithAllDataAsync(string name, int maxResultCount, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplates
            .Where(x => x.CleanName!.ToUpper() == name)
            .FilterCommunityAndPrivateItemTemplates(currentUserId, includeCommunityItems)
            .IncludeItemTemplateDetails()
            .OrderByDescending(x => x.Source)
            .Take(maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ItemTemplateEntity>> GetItemByPartialCleanNameWithAllDataAsync(string name, int maxResultCount, IEnumerable<Guid> excludedIds, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplates
            .Where(x => x.CleanName!.ToUpper().Contains(name))
            .Take(maxResultCount)
            .Where(i => !excludedIds.Contains(i.Id))
            .IncludeItemTemplateDetails()
            .FilterCommunityAndPrivateItemTemplates(currentUserId, includeCommunityItems)
            .OrderByDescending(x => x.Source)
            .ToListAsync(cancellationToken);
    }

    public Task<List<ItemTemplateEntity>> GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(string name, int maxResultCount, IEnumerable<Guid> excludedIds, int? currentUserId, bool includeCommunityItems, CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplates
            .Where(x => x.CleanName!
                .Replace("'", "")
                .Replace("-", "")
                .Replace(" ", "")
                .ToUpper()
                .Contains(name)
            )
            .Where(i => !excludedIds.Contains(i.Id))
            .FilterCommunityAndPrivateItemTemplates(currentUserId, includeCommunityItems)
            .OrderByDescending(x => x.Source)
            .Take(maxResultCount)
            .IncludeItemTemplateDetails()
            .ToListAsync(cancellationToken);
    }

    public Task<ItemTemplateEntity> GetPurseItemTemplateBasedOnMoneyAsync(int money, CancellationToken cancellationToken = default)
    {
        string techName;
        if (money <= 50)
            techName = "SMALL_PURSE";
        else if (money <= 100)
            techName = "MEDIUM_PURSE";
        else
            techName = "BIG_PURSE";

        return Context.ItemTemplates
            .Where(x => x.TechName == techName)
            .FirstAsync(cancellationToken);
    }

    public Task<ItemTemplateEntity> GetGoldCoinItemTemplate(CancellationToken cancellationToken = default)
    {
        return Context.ItemTemplates
            .Where(x => x.TechName == "MONEY")
            .FirstAsync(cancellationToken);
    }
}