using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Data.DbContexts;
using Naheulbook.Data.Models;
using Naheulbook.Data.Repositories;
using Naheulbook.TestUtils.Extensions;
using NUnit.Framework;

namespace Naheulbook.Data.Tests.Integration.Repositories
{
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
                .AddItem(out var item, character);

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
            TestDataUtil.AddItemTemplateAndRequiredData();
            TestDataUtil.AddGroupWithRequiredData();
            var user = TestDataUtil.AddUser().GetLast<UserEntity>();
            var character = TestDataUtil.AddOrigin().AddCharacter(user.Id, u => u.Group = TestDataUtil.GetLast<GroupEntity>()).GetLast<CharacterEntity>();
            TestDataUtil.AddItem(character);

            var item = await _itemRepository.GetWithOwnerAsync(TestDataUtil.Get<ItemEntity>().Id);

            var expectation = TestDataUtil.GetLast<ItemEntity>();
            item.Should().BeEquivalentTo(expectation, config => config.ExcludingChildren());
            item!.Character.Should().BeEquivalentTo(TestDataUtil.GetLast<CharacterEntity>(), config => config.ExcludingChildren());
            item!.Character!.Group.Should().BeEquivalentTo(TestDataUtil.GetLast<GroupEntity>(), config => config.ExcludingChildren());
        }

        [Test]
        public async Task GetWithOwnerAsync_ShouldLoadAllRelatedDataUsedForLootPermissionCheck()
        {
            TestDataUtil.AddItemTemplateAndRequiredData();
            TestDataUtil.AddGroupWithRequiredData();
            TestDataUtil.AddLoot();
            TestDataUtil.AddItem(TestDataUtil.GetLast<LootEntity>());

            var item = await _itemRepository.GetWithOwnerAsync(TestDataUtil.Get<ItemEntity>().Id);

            var expectation = TestDataUtil.GetLast<ItemEntity>();
            item.Should().BeEquivalentTo(expectation, config => config.ExcludingChildren());
            item!.Loot.Should().BeEquivalentTo(TestDataUtil.GetLast<LootEntity>(), config => config.ExcludingChildren());
            item!.Loot!.Group.Should().BeEquivalentTo(TestDataUtil.GetLast<GroupEntity>(), config => config.ExcludingChildren());
        }

        [Test]
        public async Task GetWithOwnerAsync_ShouldLoadAllRelatedDataUsedForMonsterPermissionCheck()
        {
            TestDataUtil.AddItemTemplateAndRequiredData();
            TestDataUtil.AddGroupWithRequiredData();
            TestDataUtil.AddMonster();
            TestDataUtil.AddItem(TestDataUtil.GetLast<MonsterEntity>());

            var item = await _itemRepository.GetWithOwnerAsync(TestDataUtil.Get<ItemEntity>().Id);

            var expectation = TestDataUtil.GetLast<ItemEntity>();
            item.Should().BeEquivalentTo(expectation, config => config.ExcludingChildren());
            item!.Monster.Should().BeEquivalentTo(TestDataUtil.GetLast<MonsterEntity>(), config => config.ExcludingChildren());
            item!.Monster!.Group.Should().BeEquivalentTo(TestDataUtil.GetLast<GroupEntity>(), config => config.ExcludingChildren());
        }
    }
}