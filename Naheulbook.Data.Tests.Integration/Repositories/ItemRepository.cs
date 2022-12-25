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
            var user = TestDataUtil.AddUser().GetLast<UserEntity>();

            TestDataUtil.AddItemTemplateWithAllData();

            var character = TestDataUtil.AddOrigin().AddCharacter(user.Id).GetLast<CharacterEntity>();

            TestDataUtil.AddItem(character);

            var item = await _itemRepository.GetWithAllDataAsync(TestDataUtil.Get<ItemEntity>().Id);

            item.Should().BeEquivalentTo(TestDataUtil.GetLast<ItemEntity>(), config => config.ExcludingChildren());
            var itemTemplate = item!.ItemTemplate;
            itemTemplate.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplateEntity>(), config => config.ExcludingChildren());
            itemTemplate.Modifiers.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplateEntity>().Modifiers, config => config.ExcludingChildren());
            itemTemplate.Requirements.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplateEntity>().Requirements, config => config.ExcludingChildren());
            itemTemplate.Skills.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplateEntity>().Skills, config => config.ExcludingChildren());
            itemTemplate.UnSkills.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplateEntity>().UnSkills, config => config.ExcludingChildren());
            itemTemplate.SkillModifiers.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplateEntity>().SkillModifiers, config => config.ExcludingChildren());
            itemTemplate.Slots.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplateEntity>().Slots, config => config.ExcludingChildren());
            itemTemplate.Slots.First().Slot.Should().BeEquivalentTo(TestDataUtil.Get<ItemTemplateEntity>().Slots.First().Slot);
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