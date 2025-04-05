using System.Linq;
using System.Threading.Tasks;
using Naheulbook.Data.EntityFrameworkCore.DbContexts;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Data.Repositories;
using Naheulbook.Shared.Extensions;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories;

public class ItemRepositoryTests : RepositoryTestsBase<NaheulbookDbContext>
{
    private ItemRepository _itemRepository;

    [SetUp]
    public void SetUp()
    {
        _itemRepository = new ItemRepository(RepositoryDbContext);
    }

    [Test]
    public async Task GetWithAllDataAsync_ShouldLoadAllRelatedData()
    {
        TestDataUtil
            .AddUser()
            .AddStat()
            .AddOrigin()
            .AddJob()
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
            .AddItemTemplateSlot(out var itemTemplateSlot)
            .AddCharacter(out var character)
            .AddItemToCharacter(out var item, character);

        var actualItem = await _itemRepository.GetWithAllDataAsync(item.Id);

        AssertEntityIsLoaded(actualItem, item);
        AssertEntityIsLoaded(actualItem.ItemTemplate, itemTemplate);
        AssertEntitiesAreLoaded(actualItem.ItemTemplate.Requirements, new[] {itemTemplateRequirement});
        AssertEntitiesAreLoaded(actualItem.ItemTemplate.Modifiers, new[] {itemTemplateModifier});
        AssertEntitiesAreLoaded(actualItem.ItemTemplate.Skills, new[] {itemTemplateSkill});
        AssertEntitiesAreLoaded(actualItem.ItemTemplate.UnSkills, new[] {itemTemplateUnSkill});
        AssertEntitiesAreLoaded(actualItem.ItemTemplate.SkillModifiers, new[] {itemTemplateSkillModifier});
        AssertEntitiesAreLoaded(actualItem.ItemTemplate.Slots, new[] {itemTemplateSlot});
        AssertEntityIsLoaded(actualItem.ItemTemplate.Slots.Single().Slot, itemSlot);
    }

    [Test]
    public async Task GetWithOwnerAsync_ShouldLoadAllRelatedDataUsedForCharacterPermissionCheck()
    {
        TestDataUtil
            .AddUser()
            .AddItemTemplateAndRequiredData()
            .AddGroup(out var group)
            .AddCharacterWithRequiredDependencies(out var character, u => u.GroupId = group.Id)
            .AddItemToCharacter(out var item, character);

        var actualItem = await _itemRepository.GetWithOwnerAsync(TestDataUtil.Get<ItemEntity>().Id);

        AssertEntityIsLoaded(actualItem, item);
        AssertEntityIsLoaded(actualItem.Character, item.Character.NotNull());
        AssertEntityIsLoaded(actualItem.Character.Group, item.Character.Group.NotNull());
    }

    [Test]
    public async Task GetWithOwnerAsync_ShouldLoadAllRelatedDataUsedForLootPermissionCheck()
    {
        TestDataUtil
            .AddUser()
            .AddItemTemplateAndRequiredData()
            .AddGroup(out var group)
            .AddLoot(out var loot)
            .AddItemToLoot(out var item, loot);

        var actualItem = await _itemRepository.GetWithOwnerAsync(TestDataUtil.Get<ItemEntity>().Id);

        AssertEntityIsLoaded(actualItem, item);
        AssertEntityIsLoaded(actualItem.Loot, loot);
        AssertEntityIsLoaded(actualItem.Loot.Group, group);
    }

    [Test]
    public async Task GetWithOwnerAsync_ShouldLoadAllRelatedDataUsedForMonsterPermissionCheck()
    {
        TestDataUtil
            .AddUser()
            .AddItemTemplateAndRequiredData()
            .AddGroup(out var group)
            .AddMonster(out var monster)
            .AddItemToMonster(out var item, TestDataUtil.GetLast<MonsterEntity>());

        var actualItem = await _itemRepository.GetWithOwnerAsync(TestDataUtil.Get<ItemEntity>().Id);

        AssertEntityIsLoaded(actualItem, item);
        AssertEntityIsLoaded(actualItem.Monster, monster);
        AssertEntityIsLoaded(actualItem.Monster.Group, group);
    }
}