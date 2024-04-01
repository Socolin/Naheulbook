#pragma warning disable 8619
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Extensions;
using Naheulbook.Data.Models;

namespace Naheulbook.Data.Repositories;

public interface IItemTemplateRepository : IRepository<ItemTemplateEntity>
{
    Task<ItemTemplateEntity?> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(Guid id);
    Task<List<ItemTemplateEntity>> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(int sectionId);
    Task<List<ItemTemplateEntity>> GetWithAllDataByCategoryIdAsync(int subCategoryId, int? currentUserId, bool includeCommunityItems);
    Task<List<ItemTemplateEntity>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<List<ItemTemplateEntity>> GetItemByCleanNameWithAllDataAsync(string name, int maxResultCount, int? currentUserId, bool includeCommunityItems);
    Task<List<ItemTemplateEntity>> GetItemByPartialCleanNameWithAllDataAsync(string name, int maxResultCount, IEnumerable<Guid> excludedIds, int? currentUserId, bool includeCommunityItems);
    Task<List<ItemTemplateEntity>> GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(string name, int maxResultCount, IEnumerable<Guid> excludedIds, int? currentUserId, bool includeCommunityItems);
    Task<ItemTemplateEntity> GetPurseItemTemplateBasedOnMoneyAsync(int money);
    Task<ItemTemplateEntity> GetGoldCoinItemTemplate();
}

public class ItemTemplateRepository(NaheulbookDbContext context) : Repository<ItemTemplateEntity, NaheulbookDbContext>(context), IItemTemplateRepository
{
    public Task<ItemTemplateEntity?> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(Guid id)
    {
        return Context.ItemTemplates
            .IncludeItemTemplateDetails()
            .Where(x => x.Id == id)
            .SingleOrDefaultAsync();
    }

    public Task<List<ItemTemplateEntity>> GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync(int sectionId)
    {
        return Context.ItemTemplates
            .IncludeItemTemplateDetails()
            .Where(x => x.SubCategory.SectionId == sectionId)
            .ToListAsync();
    }

    public Task<List<ItemTemplateEntity>> GetWithAllDataByCategoryIdAsync(int subCategoryId, int? currentUserId, bool includeCommunityItems)
    {
        return Context.ItemTemplates
            .IncludeItemTemplateDetails()
            .FilterCommunityAndPrivateItemTemplates(currentUserId, includeCommunityItems)
            .Where(x => x.SubCategoryId == subCategoryId)
            .ToListAsync();
    }

    public Task<List<ItemTemplateEntity>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return Context.ItemTemplates
            .IncludeItemTemplateDetails()
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
    }

    public Task<List<ItemTemplateEntity>> GetItemByCleanNameWithAllDataAsync(string name, int maxResultCount, int? currentUserId, bool includeCommunityItems)
    {
        return Context.ItemTemplates
            .Where(x => x.CleanName!.ToUpper() == name)
            .FilterCommunityAndPrivateItemTemplates(currentUserId, includeCommunityItems)
            .IncludeItemTemplateDetails()
            .OrderByDescending(x => x.Source)
            .Take(maxResultCount)
            .ToListAsync();
    }

    public Task<List<ItemTemplateEntity>> GetItemByPartialCleanNameWithAllDataAsync(string name, int maxResultCount, IEnumerable<Guid> excludedIds, int? currentUserId, bool includeCommunityItems)
    {
        return Context.ItemTemplates
            .Where(x => x.CleanName!.ToUpper().Contains(name))
            .Take(maxResultCount)
            .Where(i => !excludedIds.Contains(i.Id))
            .IncludeItemTemplateDetails()
            .FilterCommunityAndPrivateItemTemplates(currentUserId, includeCommunityItems)
            .OrderByDescending(x => x.Source)
            .ToListAsync();
    }

    public Task<List<ItemTemplateEntity>> GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync(string name, int maxResultCount, IEnumerable<Guid> excludedIds, int? currentUserId, bool includeCommunityItems)
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
            .ToListAsync();
    }

    public Task<ItemTemplateEntity> GetPurseItemTemplateBasedOnMoneyAsync(int money)
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
            .FirstAsync();
    }

    public Task<ItemTemplateEntity> GetGoldCoinItemTemplate()
    {
        return Context.ItemTemplates
            .Where(x => x.TechName == "MONEY")
            .FirstAsync();
    }
}