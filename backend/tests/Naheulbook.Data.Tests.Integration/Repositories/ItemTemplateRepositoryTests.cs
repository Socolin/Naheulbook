using FluentAssertions;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.Repositories;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class ItemTemplateRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private ItemTemplateRepository _itemTemplateRepository;

    [SetUp]
    public void SetUp()
    {
        _itemTemplateRepository = new ItemTemplateRepository(RepositoryDbContext);
    }

    #region GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync

    [Test]
    public async Task GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync_LoadEntityWithRelatedEntities()
    {
        TestDataUtil
            .AddStat()
            .AddSkill(out var learntSkill)
            .AddSkill(out var cancelledSkill)
            .AddSkill(out var modifiedSkill)
            .AddItemTemplateSection()
            .AddItemTemplateSubCategory()
            .AddSlot(out var itemSlot)
            .AddItemTemplate(out var itemTemplate)
            .AddItemTemplateRequirement(out var itemTemplateRequirement)
            .AddItemTemplateModifier(out var itemTemplateModifier)
            .AddItemTemplateSkill(out var itemTemplateSkill, learntSkill)
            .AddItemTemplateUnSkill(out var itemTemplateUnSkill, cancelledSkill)
            .AddItemTemplateSkillModifier(out var itemTemplateSkillModifier, modifiedSkill)
            .AddItemTemplateSlot(out var itemTemplateSlot);

        var actual = await _itemTemplateRepository.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplate.Id);

        AssertEntityIsLoaded(actual, itemTemplate);
        AssertEntitiesAreLoaded(actual.Requirements, new[] {itemTemplateRequirement});
        AssertEntitiesAreLoaded(actual.Modifiers, new[] {itemTemplateModifier});
        AssertEntitiesAreLoaded(actual.Skills, new[] {itemTemplateSkill});
        AssertEntitiesAreLoaded(actual.UnSkills, new[] {itemTemplateUnSkill});
        AssertEntitiesAreLoaded(actual.SkillModifiers, new[] {itemTemplateSkillModifier});
        AssertEntitiesAreLoaded(actual.Slots, new[] {itemTemplateSlot});
        AssertEntityIsLoaded(actual.Slots.Single().Slot, itemSlot);
    }

    [Test]
    public async Task GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync_ReturnsNullWhenNotfound()
    {
        TestDataUtil
            .AddStat()
            .AddItemTemplateSection()
            .AddItemTemplateSubCategory()
            .AddItemTemplate(out var itemTemplate);

        var actualItemTemplate = await _itemTemplateRepository.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(Guid.NewGuid());
        var existingItemTemplate = await _itemTemplateRepository.GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsAsync(itemTemplate.Id);

        actualItemTemplate.Should().BeNull();
        existingItemTemplate.Should().NotBeNull();
    }

    #endregion

    #region GetWithModifiersWithRequirementsWithSkillsWithSkillModifiersWithSlotsWithUnSkillsBySectionIdAsync

    #endregion

    #region GetWithAllDataByCategoryIdAsync

    #endregion

    #region GetByIdsAsync

    [Test]
    public async Task GetByIdsAsync_LoadRelatedEntities()
    {
        TestDataUtil
            .AddStat()
            .AddSkill(out var learntSkill)
            .AddSkill(out var cancelledSkill)
            .AddSkill(out var modifiedSkill)
            .AddItemTemplateSection()
            .AddItemTemplateSubCategory()
            .AddSlot(out var itemSlot)
            .AddItemTemplate(out var itemTemplate)
            .AddItemTemplateRequirement(out var itemTemplateRequirement)
            .AddItemTemplateModifier(out var itemTemplateModifier)
            .AddItemTemplateSkill(out var itemTemplateSkill, learntSkill)
            .AddItemTemplateUnSkill(out var itemTemplateUnSkill, cancelledSkill)
            .AddItemTemplateSkillModifier(out var itemTemplateSkillModifier, modifiedSkill)
            .AddItemTemplateSlot(out var itemTemplateSlot);

        var actualEntities = await _itemTemplateRepository.GetByIdsAsync(new[] {itemTemplate.Id});

        var actualItemTemplate = actualEntities.Single();
        AssertEntityIsLoaded(actualItemTemplate, itemTemplate);
        AssertEntitiesAreLoaded(actualItemTemplate.Requirements, new[] {itemTemplateRequirement});
        AssertEntitiesAreLoaded(actualItemTemplate.Modifiers, new[] {itemTemplateModifier});
        AssertEntitiesAreLoaded(actualItemTemplate.Skills, new[] {itemTemplateSkill});
        AssertEntitiesAreLoaded(actualItemTemplate.UnSkills, new[] {itemTemplateUnSkill});
        AssertEntitiesAreLoaded(actualItemTemplate.SkillModifiers, new[] {itemTemplateSkillModifier});
        AssertEntitiesAreLoaded(actualItemTemplate.Slots, new[] {itemTemplateSlot});
        AssertEntityIsLoaded(actualItemTemplate.Slots.Single().Slot, itemSlot);
    }

    [Test]
    public async Task GetByIdsAsync_LoadAllEntitiesMatchingGivenIds()
    {
        TestDataUtil
            .AddItemTemplateSection()
            .AddItemTemplateSubCategory()
            .AddItemTemplate(out var itemTemplate1)
            .AddItemTemplate(out var itemTemplate2)
            .AddItemTemplate();

        var actualEntities = await _itemTemplateRepository.GetByIdsAsync(new[] {itemTemplate1.Id, itemTemplate2.Id});

        AssertEntitiesAreLoaded(actualEntities, new[] {itemTemplate1, itemTemplate2});
    }

    #endregion

    #region GetItemByCleanNameWithAllDataAsync

    #endregion

    #region GetItemByPartialCleanNameWithAllDataAsync

    #endregion

    #region GetItemByPartialCleanNameWithoutSeparatorWithAllDataAsync

    #endregion

    #region GetPurseItemTemplateBasedOnMoneyAsync

    #endregion

    #region GetGoldCoinItemTemplate

    #endregion
}